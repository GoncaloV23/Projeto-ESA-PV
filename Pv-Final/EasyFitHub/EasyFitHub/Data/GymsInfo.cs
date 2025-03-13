using EasyFitHub.Models.Account;
using EasyFitHub.Models.Gym;
using EasyFitHub.Models.Miscalenous;
using EasyFitHub.Models.Profile;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace EasyFitHub.Data
{
    /// <summary>
    /// AUTHOR:André P.
    /// Usado para realizar operaçpões CRUD em ginásios
    /// </summary>
    public class GymsInfo
    {
        private readonly EasyFitHubContext _context;
        private readonly ILogger? _logger;
        public GymsInfo(EasyFitHubContext context, ILogger? logger = null)
        {
            _context = context;
            _logger = logger;
        }
        /// <summary>
        /// Removes the relation between an employee and a client from a gym.
        /// </summary>
        /// <param name="employee">The employee client.</param>
        /// <param name="client">The client to be removed.</param>
        /// <param name="gym">The gym from which the relation is to be removed.</param>
        /// <returns>True if the relation is successfully removed, otherwise false.</returns>

        public async Task<bool> RemoveEmployeeClientRelation(Client employee, Client client, Gym gym)
        {
            try
            {
                var relation = await _context.GymRelations.SingleOrDefaultAsync
                ( r => 
                    r.GymClient.ClientId == client.ClientId &&
                    r.GymEmployee.ClientId == employee.ClientId
                );

                if (relation == null) return false;


                var res = _context.GymRelations.Remove( relation );
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                printMessage($"Error in RemoveEmployeeClientRelation of GymsInfo: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// Adds a relation between an employee and a client to a gym.
        /// </summary>
        /// <param name="employee">The employee client.</param>
        /// <param name="client">The client to be added.</param>
        /// <param name="gym">The gym to which the relation is to be added.</param>
        /// <returns>True if the relation is successfully added, otherwise false.</returns>
        public async Task<bool> AddEmployeeClientRelation(Client employee, Client client, Gym gym)
        {
            try
            {
                var dbEmploye = await _context.GymEmployees.SingleOrDefaultAsync(e => e.GymId == gym.Id && e.ClientId == employee.ClientId);
                if (dbEmploye == null) return false;

                var dbClient = await _context.GymClients.SingleOrDefaultAsync(c => c.GymId == gym.Id && c.ClientId == client.ClientId);
                if (dbClient == null) return false;

                var relation = await _context.GymRelations.SingleOrDefaultAsync
                (r =>
                    r.GymClient.ClientId == client.ClientId &&
                    r.GymEmployee.ClientId == employee.ClientId
                );
                if (relation != null) return false;


                dbEmploye.GymClients.Add(
                    new GymRelation 
                    {
                        GymClient = dbClient, 
                        GymEmployee = dbEmploye
                    } 
                );
                
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                printMessage($"Error in AddEmployeeClientRelation of GymsInfo: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// Retrieves the relation between a client and a gym.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="gym">The gym.</param>
        /// <returns>The relation between the client and the gym, or null if not found.</returns>
        public async Task<GymClient?> ClientGymRelation(Client client, Gym gym)
        {
            try
            {
                var relataion = await _context.GymClients.SingleOrDefaultAsync(gc => gc.GymId == gym.Id && gc.ClientId == client.ClientId);
                if (relataion != null) { return relataion; }
                return null;
            }
            catch (Exception ex)
            {
                printMessage($"Error in ClientGymRelation of GymsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Retrieves the relation between an employee and a gym.
        /// </summary>
        /// <param name="employee">The employee client.</param>
        /// <param name="gym">The gym.</param>
        /// <returns>The relation between the employee and the gym, or null if not found.</returns>
        public async Task<GymEmployee?> EmployeeGymRelation(Client employee, Gym gym)
        {
            try
            {
                var relataion = gym.GymEmployees.FirstOrDefault(g => g.ClientId == employee.ClientId);
                if (relataion != null) { return relataion; }
                return null;
            }
            catch (Exception ex)
            {
                printMessage($"Error in EmployeeGymRelation of GymsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Retrieves a gym by its ID.
        /// </summary>
        /// <param name="id">The ID of the gym to retrieve.</param>
        /// <returns>The gym with the specified ID, or null if not found.</returns>
        public async ValueTask<Gym?> GetGym(int id)
        {
            try
            {
                return await _context.Gym
                    .Include(g => g.Requests).ThenInclude(r => r.Client).ThenInclude(c => c.User)
                    .Include(g => g.GymClients).ThenInclude(gc => gc.Client).ThenInclude(c => c.User)
                    .Include(g => g.GymClients).ThenInclude(gc => gc.Client).ThenInclude(c => c.Data)
                    .Include(g => g.GymEmployees).ThenInclude(ge => ge.Client).ThenInclude(c => c.User)
                    .Include(g => g.GymEmployees).ThenInclude(ge => ge.Client).ThenInclude(c => c.Data)
                    .Include(g => g.Images)
                    .Include(g => g.Items)
                    .Include(g => g.Machines)
                    .FirstOrDefaultAsync(g => g.Id == id);
                }
            catch (Exception ex)
            {
                printMessage($"Error in GetGym of GymsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Retrieves a gym by its name.
        /// </summary>
        /// <param name="gymName">The name of the gym to retrieve.</param>
        /// <returns>The gym with the specified name, or null if not found.</returns>
        public async Task<Gym?> GetGym(string gymName)
        {
            try
            {
                var gym = await _context.Gym.FirstOrDefaultAsync(g => g.Name == gymName);
                _context.Entry(gym).Collection(g => g.Requests).Load();

                return gym;

            }
            catch (Exception ex)
            {
                printMessage($"Error in GetGym of GymsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Updates information about a gym.
        /// </summary>
        /// <param name="newGym">The updated gym information.</param>
        /// <returns>The updated gym, or null if not found.</returns>
        public async Task<Gym?> UpdateGym(Gym newGym)
        {
            try
            {
                Gym? gym = await _context.Gym.SingleOrDefaultAsync(g => g.Id == newGym.Id);

                if (gym == null) return null;

                gym.Description = newGym.Description;
                gym.Location = newGym.Location;
                gym.Name = newGym.Name;

                await _context.SaveChangesAsync();

                return gym;
            }
            catch (Exception ex)
            {
                printMessage($"Error in UpdateGym of GymsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Adds a request for a client to join a gym.
        /// </summary>
        /// <param name="gym">The gym.</param>
        /// <param name="client">The client requesting to join the gym.</param>
        /// <returns>The added gym request, or null if the gym or client is not found.</returns>
        public async ValueTask<GymRequest?> AddRequest(Gym gym, Client client)
        {
            try
            {
                if (gym == null || client == null) return null;

                var queryGym = await _context.Gym.FindAsync(gym.Id);

                if (queryGym == null) return null;

                var req = new GymRequest { Client = client };
                queryGym.Requests.Add(req);

                await _context.SaveChangesAsync();


                return req;
            }
            catch (Exception ex)
            {
                printMessage($"Error in AddRequest of GymsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Removes a request for a client to join a gym.
        /// </summary>
        /// <param name="gym">The gym.</param>
        /// <param name="client">The client whose request is to be removed.</param>
        /// <returns>The removed gym request, or null if not found.</returns>
        public async ValueTask<GymRequest?> RemoveRequest(Gym gym, Client client)
        {
            try
            {
                if (gym == null || client == null) return null;

                var queryGym = await _context.Gym.FindAsync(gym.Id);

                if (queryGym == null || client == null) return null;

                var req = queryGym.Requests.SingleOrDefault(r => r.ClientId == client.ClientId);
                if(req == null)return null;

                _context.GymRequests.Remove(req);

                await _context.SaveChangesAsync();

                return req;
            }
            catch (Exception ex)
            {
                printMessage($"Error in RemoveRequest of GymsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Retrieves the client associated with a user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>The client associated with the user, or null if not found.</returns>
        public async ValueTask<Client?> GetClient(User user)
        {
            try
            {
                if (user == null) return null;
                return await _context.Client.SingleAsync(client => client.UserId == user.AccountId);
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetClient of GymsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Adds an employee to a gym.
        /// </summary>
        /// <param name="client">The client to be added as an employee.</param>
        /// <param name="gym">The gym.</param>
        /// <param name="role">The role of the employee.</param>
        /// <returns>The added gym employee, or null if the gym or client already exists as an employee or client.</returns>
        public async Task<GymEmployee?> AddEmployee(Client client, Gym gym, Role role)
        {
            try
            {
                GymEmployee employee = new GymEmployee
                {
                    Gym = gym,
                    Client = client,
                    EnrollmentDate = DateTime.UtcNow,
                    Role = role
                };

                if (gym == null || 
                    gym.GymEmployees.Any(e => e.ClientId == client.ClientId) || 
                    gym.GymClients.Any(e => e.ClientId == client.ClientId))
                {
                    return null;
                }

                gym.GymEmployees.Add(employee);
                await _context.SaveChangesAsync();

                await RemoveRequest(gym, client);

                return employee;
            }
            catch (Exception ex)
            {
                printMessage($"Error in AddEmployee of GymsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Adds a client to a gym.
        /// </summary>
        /// <param name="client">The client to be added.</param>
        /// <param name="gym">The gym.</param>
        /// <returns>The added gym client, or null if the gym or client already exists as an employee or client.</returns>
        public async Task<GymClient?> AddClient(Client client, Gym gym)
        {
            try
            {
                GymClient newClient = new GymClient
                {
                    Gym = gym,
                    Client = client,
                    EnrollmentDate = DateTime.UtcNow,
                    Role = Role.CLIENT
                };


                if (gym == null ||
                    gym.GymEmployees.Any(e => e.ClientId == client.ClientId) ||
                    gym.GymClients.Any(e => e.ClientId == client.ClientId))
                {
                    return null;
                }

                gym.GymClients.Add(newClient);
                await _context.SaveChangesAsync();

                await RemoveRequest(gym, client);

                return newClient;
            }
            catch (Exception ex)
            {
                printMessage($"Error in AddClient of GymsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Removes a client from a gym.
        /// </summary>
        /// <param name="client">The client to be removed.</param>
        /// <param name="gym">The gym.</param>
        /// <returns>The removed gym client, or null if not found.</returns>
        public async Task<GymClient?> RemoveClient(Client client, Gym gym)
        {
            try
            {
                var rel = await ClientGymRelation(client, gym);
                if (rel != null)
                {
                    gym.GymClients.Remove(rel);
                    await _context.SaveChangesAsync();
                    return rel;
            }
            return rel;
            }
            catch (Exception ex)
            {
                printMessage($"Error in RemoveClient of GymsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Removes an employee from a gym.
        /// </summary>
        /// <param name="employee">The employee client to be removed.</param>
        /// <param name="gym">The gym.</param>
        /// <returns>The removed gym employee, or null if not found.</returns>
        public async Task<GymEmployee?> RemoveEmployee(Client employee, Gym gym)
        {
            try
            {
                var rel = await EmployeeGymRelation(employee, gym);
                if (rel != null)
                {
                    gym.GymEmployees.Remove(rel);
                    await _context.SaveChangesAsync();
                    return rel;
                }
                return rel;
            }
            catch (Exception ex)
            {
                printMessage($"Error in RemoveEmployee of GymsInfo: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Retrieves a gym employee by their ID.
        /// </summary>
        /// <param name="gymEmployId">The ID of the gym employee.</param>
        /// <returns>The gym employee with the specified ID, or null if not found.</returns>
        public async Task<GymEmployee?> GetGymEmployee(int gymEmployId)
        {
            try
            {
                return await _context.GymEmployees
                    .Include(ge => ge.GymClients)
                    .ThenInclude(rel => rel.GymClient)
                    .SingleOrDefaultAsync(ge => ge.GymEmployeeId == gymEmployId);
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetGymEmployee of GymsInfo: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Retrieves all gyms.
        /// </summary>
        /// <returns>A collection of all gyms.</returns>
        public async Task<IEnumerable<Gym>> GetGyms()
        {
            try
            {
                return _context.Gym.AsEnumerable<Gym>();
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetGyms of GymsInfo: {ex.Message}");
                return new List<Gym>();
            }
        }

        /// <summary>
        /// Deletes a gym and associated entities.
        /// </summary>
        /// <param name="gymId">The ID of the gym to be deleted.</param>
        /// <returns>True if the gym is successfully deleted, otherwise false.</returns>
        public async Task<bool> DeleteGym(int gymId)
        {
            try
            {
                var gym = await _context.Gym.FindAsync(gymId);
                if (gym == null)
                    return false;

                var aux =  _context.Gym.Where(g => g.Id == gymId).SelectMany(g => g.GymClients);
                var employees = await _context.Gym.Where(g => g.Id == gymId).SelectMany(g => g.GymEmployees).ToListAsync();
                var relations = await aux.SelectMany(gc => gc.GymEmployees).ToListAsync();
                var clients = await aux.ToListAsync();
                var requests = await _context.Gym.Where(g => g.Id == gymId).SelectMany(g => g.Requests).ToListAsync();
                

                if (relations != null)
                    _context.GymRelations.RemoveRange(relations);

                if (employees != null)
                    _context.GymEmployees.RemoveRange(employees);

                if (clients != null)
                    _context.GymClients.RemoveRange(clients);

                if (clients != null)
                    _context.GymRequests.RemoveRange(requests);


                await _context.SaveChangesAsync();


                _context.Gym.RemoveRange(gym);
                await _context.SaveChangesAsync();
                /*
                 var gymClients = await _context.GymClients
                    .Include(gc => gc.GymEmployees)
                    .Where(gc => gc.GymId == gymId)
                    .ToListAsync();

                _context.GymClients.RemoveRange(gymClients);
                
                var gymEmployees = await _context.GymEmployees
                    .Include(ge => ge.GymClients)
                    .Where(ge => ge.GymId == gymId)
                    .ToListAsync();

                _context.GymEmployees.RemoveRange(gymEmployees);
                */
                //_context.Gym.Remove(gym);



                return true;
            }
            catch (Exception ex)
            {
                printMessage($"Error in DeleteGym of GymsInfo: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// Confirms a gym.
        /// </summary>
        /// <param name="gymId">The ID of the gym to be confirmed.</param>
        /// <returns>True if the gym is successfully confirmed, otherwise false.</returns>
        public async Task<bool> ConfirmGym(int gymId)
        {
            try
            {
                var gym = await _context.Gym.SingleOrDefaultAsync(g => g.Id == gymId);
                if (gym == null) return false;


                gym.IsConfirmed = true;

                var res = _context.Update(gym);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                printMessage($"Error in ConfirmGym of GymsInfo: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Retrieves an image associated with a gym.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="imageName">The name of the image to retrieve.</param>
        /// <returns>The retrieved image, or null if not found.</returns>
        public async Task<HubImage?> GetImage(int gymId, string imageName)
        {
            try
            {
                var data = await GetGym(gymId);
                if (data == null) return null;

                var img = data!.Images!.SingleOrDefault(i => i.Name == imageName);
                if (img == null) return null;

                return img;
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetImage of GymsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Creates an image associated with a gym.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <returns>The created image, or null if the gym is not found.</returns>
        public async Task<HubImage?> CreateImage(int gymId)
        {
            try
            {
                var data = await GetGym(gymId);
                if (data == null) return null;

                var name = "image_gym" + data.Id + Guid.NewGuid().ToString();

                data.Images.Add(new HubImage
                {
                    Description = "No description",
                    Name = name,
                    Path = ""
                });

                await _context.SaveChangesAsync();

                return data.Images.Last();
            }
            catch (Exception ex)
            {
                printMessage($"Error in CreateImage of GymsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Updates the path of an image associated with a gym.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="imageName">The name of the image.</param>
        /// <param name="newPath">The new path of the image.</param>
        /// <returns>The updated image, or null if the gym or image is not found.</returns>
        public async Task<HubImage?> UpdatePathImage(int gymId, string imageName, string newPath)
        {
            try
            {
                var data = await GetGym(gymId);
                if (data == null) return null;


                var img = data!.Images!.SingleOrDefault(i => i.Name == imageName);

                if (img == null) return null;
                img.Path = newPath;

                await _context.SaveChangesAsync();

                return img;
            }
            catch (Exception ex)
            {
                printMessage($"Error in UpdatePathImage of GymsInfo: {ex.Message}");
                return null;
            }
        }
/// <summary>
/// Removes an image associated with a gym.
/// </summary>
/// <param name="gymId">The ID of the gym.</param>
/// <param name="imageName">The name of the image to remove.</param>
/// <returns>The removed image, or null if not found.</returns>
        public async Task<HubImage?> RemoveImage(int gymId, string imageName)
        {
            try
            {
                var data = await GetGym(gymId);
                if (data == null) return null;

                var img = data!.Images!.SingleOrDefault(i => i.Name == imageName);
                if (img == null) return null;

                _context.Images.Remove(img);
                await _context.SaveChangesAsync();

                return img;
            }
            catch (Exception ex)
            {
                printMessage($"Error in RemoveImage of GymsInfo: {ex.Message}");
                return null;
            }
        }






        private void printMessage(string message)
        {
            if (_logger == null)
                Console.WriteLine($"\n\n\n\n{message}\n\n\n\n");
            else
                _logger.LogError($"\n\n\n\n{message}\n\n\n\n");
        }
    }
}

