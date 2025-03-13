using EasyFitHub.Models.Account;
using EasyFitHub.Models.Miscalenous;
using EasyFitHub.Models.Profile;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace EasyFitHub.Data
{
    /// <summary>
    /// AUTHOR:André P.
    /// Destinado a efetuar operaçoes no contexto referentes aos perfis dos Client's
    /// </summary>
    public class ProfilesInfo
    {
        private readonly EasyFitHubContext _context;
        private readonly ILogger? _logger;
        public ProfilesInfo(EasyFitHubContext context, ILogger? logger = null)
        {
            _context = context;
            _logger = logger;
        }
        /// <summary>
        /// Retrieves a user by their ID, including related entities.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>Returns the user with related data if found, otherwise null.</returns>
        public Client? GetUser(int id)
        {
            try
            {
                var query = _context.Client
                    .Include(c => c.Data).ThenInclude(d => d.Image)
                    .Include(c => c.Biometrics)
                    .Include(c => c.User)
                    .FirstOrDefault(u => u.UserId == id);
                return query;
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetUser of ProfilesInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Retrieves client data associated with a client.
        /// </summary>
        /// <param name="client">The client whose data is to be retrieved.</param>
        /// <returns>Returns the client data if found, otherwise null.</returns>
        public ClientData? GetClientData(Client client)
        {
            try
            {
                var theClient = _context.Client.Include(c => c.Data).FirstOrDefault(c => c.ClientId == client.ClientId);
                if (theClient == null) return null;
                return theClient.Data;
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetClientData of ProfilesInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Retrieves a list of clients in the application.
        /// </summary>
        /// <returns>Returns a list of clients.</returns>
        public List<User> GetClients()
        {
            try
            {
                return _context.Client.Include(c => c.User).Where(w => w.User.AccountType == AccountType.USER).Select(c => c.User).ToList();
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetClients of ProfilesInfo: {ex.Message}");
                return new List<User>();
            }
        }
        /// <summary>
        /// Retrieves a list of employees (managers) in the application.
        /// </summary>
        /// <returns>Returns a list of employees.</returns>
        public List<User> GetEmployees()
        {
            try
            {
                return _context.Client.Include(c => c.User).Where(w => w.User.AccountType == AccountType.MANAGER).Select(c => c.User).ToList();
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetEmployees of ProfilesInfo: {ex.Message}");
                return new List<User>();
            }
        }

        /// <summary>
        /// Retrieves clients associated with a specific client.
        /// </summary>
        /// <param name="client">The client whose associated clients are to be retrieved.</param>
        /// <returns>Returns a list of clients associated with the specified client.</returns>
        public List<Client> GetClients(Client client)
        {
            try
            {
                var gyms = _context.Gym
                    .Where(g => g.GymEmployees.Any(gc => gc.ClientId == client.ClientId));

                List<Client> clients = gyms
                    .SelectMany(g => g.GymClients.Where(ge => ge.GymEmployees.Any(c => c.GymEmployee.ClientId == client.ClientId)))
                    .Distinct()
                    .Select(ge => ge.Client)
                    .ToList();

                return clients;
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetClients of ProfilesInfo: {ex.Message}");
                
                return new List<Client>();
            }
        }

        /// <summary>
        /// Deletes a user from the database.
        /// </summary>
        /// <param name="user">The user to be deleted.</param>
        /// <returns>Returns true if the user is deleted successfully, otherwise false.</returns>
        public bool DeleteUser(User user)
        {
            try
            {
                var query = _context.Client.Include(c => c.User).FirstOrDefault(u => u.UserId == user.AccountId);
                if (query != null)
                {
                    _context.Attach(query.User);
                    _context.Remove(query.User);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                printMessage($"Error in DeleteUser of ProfilesInfo: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// Retrieves employees associated with a specific client.
        /// </summary>
        /// <param name="client">The client whose associated employees are to be retrieved.</param>
        /// <returns>Returns a list of employees associated with the specified client.</returns>
        public List<Client> GetEmployees(Client client)
        {
            try
            {
                var gyms = _context.Gym
                    .Where(g => g.GymClients.Any(gc => gc.ClientId == client.ClientId));

                List<Client> employees = gyms
                    .SelectMany(g => g.GymEmployees
                    .Where(ge => ge.GymClients.Any(c => c.GymClient.ClientId == client.ClientId)))
                    .Distinct()
                    .Select(ge => ge.Client)
                    .ToList();

                return employees;
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetEmployees of ProfilesInfo: {ex.Message}");
                return new List<Client>();
            }
        }
        
        /// <summary>
        /// updates a Client's information
        /// </summary>
        /// <param name="clientId">id of the Client</param>
        /// <param name="client">the new Client info</param>
        /// <returns>True if the update is sucessful; False if there is a failure</returns>
        public async Task<bool> UpdateClient(int clientId, Client client)
        {
            try
            {
                var dbClient = await _context.Client.SingleOrDefaultAsync(c => c.ClientId == clientId);
                if (dbClient == null) return false;

                dbClient.Description = client.Description;
                dbClient.Gender = client.Gender;

                var res = true;

                if (client.User != null) { res = res && await UpdateUser(clientId, client.User); }
                if (client.Biometrics != null) { res = res && await UpdateBiometrics(clientId, client.Biometrics); }
                if (client.Data != null) { res = res && await UpdateClientData(clientId, client.Data); }

                return res;
            }
            catch (Exception ex)
            {
                printMessage($"Error in UpdateClient of ProfilesInfo: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// Updates a client's biometrics information in the database.
        /// </summary>
        /// <param name="clientId">The ID of the client whose biometrics are to be updated.</param>
        /// <param name="biometrics">The updated biometrics data.</param>
        /// <returns>Returns true if the biometrics are updated successfully, otherwise false.</returns>
        public async Task<bool> UpdateBiometrics(int clientId, Biometrics biometrics)
        {
            try
            {
                var dbBiometric = await _context.Client.Where(c => c.ClientId == clientId).Select(c => c.Biometrics).FirstAsync();
                if (dbBiometric == null) return false;

                dbBiometric.WaterPercentage = biometrics.WaterPercentage;
                dbBiometric.Weigth = biometrics.Weigth;
                dbBiometric.Height = biometrics.Height;
                dbBiometric.WaterPercentage = biometrics.WaterPercentage;
                dbBiometric.LeanMass = biometrics.LeanMass;
                dbBiometric.BodyMassIndex = biometrics.BodyMassIndex;
                dbBiometric.MetabolicAge = biometrics.MetabolicAge;
                dbBiometric.VisceralFat = biometrics.VisceralFat;
                dbBiometric.FatMass = biometrics.FatMass;

                await _context.SaveChangesAsync();

                return true;    
            }
            catch (Exception ex)
            {
                printMessage($"Error in UpdateBiometrics of ProfilesInfo: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// Updates a user's information in the database.
        /// </summary>
        /// <param name="clientId">The ID of the client whose user information is to be updated.</param>
        /// <param name="user">The updated user object.</param>
        /// <returns>Returns true if the user information is updated successfully, otherwise false.</returns>
        public async Task<bool> UpdateUser(int clientId, User user)
        {
            try
            {
                var dbUser = await _context.Client.Where(c => c.ClientId == clientId).Select(c => c.User).FirstAsync();
                if (dbUser == null) return false;

                dbUser.Name = user.Name;
                dbUser.Surname = user.Surname;
                dbUser.BirthDate = user.BirthDate;
                dbUser.UserName = user.UserName;
                dbUser.Email = user.Email;
                dbUser.PhoneNumber = user.PhoneNumber;


                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                printMessage($"Error in UpdateUser of ProfilesInfo: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// Updates a client's data in the database.
        /// </summary>
        /// <param name="clientId">The ID of the client whose data is to be updated.</param>
        /// <param name="data">The updated client data object.</param>
        /// <returns>Returns true if the client data is updated successfully, otherwise false.</returns>
        public async Task<bool> UpdateClientData(int clientId, ClientData data)
        {
            try
            {
                var dbData = await _context.Client.Where(c => c.ClientId == clientId).Select(c => c.Data).FirstAsync();
                if (dbData == null) return false;

                dbData.Location = data.Location;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                printMessage($"Error in UpdateClientData of ProfilesInfo: {ex.Message}");
                return false;
            }
        }



        /// <summary>
        /// Creates a new image for the client's profile.
        /// </summary>
        /// <param name="client">The client for whom the image is created.</param>
        /// <returns>Returns the created image if successful, otherwise null.</returns>
        public async Task<HubImage?> CreateImage(Client client)
        {
            try
            {
                var data = await _context.ClientData.FirstOrDefaultAsync(cd => cd.ClientDataId == client.ClientDataId);

                if (data == null) return null;

                var name = "image_profile" + client.ClientId + client.ClientDataId;
                if (data.Image == null)
                {
                    var img = new HubImage
                    {
                        Description = "No description",
                        Name = name,
                        Path = "/lib/imagens/ProfileV2.PNG"
                    };
                    data.Image = img;
                }
                else
                {
                    data.Image.Name = name;
                }

                await _context.SaveChangesAsync();

                return data.Image;
            }
            catch (Exception ex)
            {
                printMessage($"Error in CreateImage of ProfilesInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Updates the path of an image associated with the client's profile.
        /// </summary>
        /// <param name="client">The client whose image path is to be updated.</param>
        /// <param name="imageName">The name of the image.</param>
        /// <param name="newPath">The new path for the image.</param>
        /// <returns>Returns the updated image object if successful, otherwise null.</returns>
        public async Task<HubImage?> UpdatePathImage(Client client, string imageName, string newPath)
        {
            try
            {
                var data = await _context.ClientData.FirstOrDefaultAsync(cd => cd.ClientDataId == client.ClientDataId);

                var img = data!.Image;
                img.Path = newPath;

                await _context.SaveChangesAsync();

                return img;
            }
            catch (Exception ex)
            {
                printMessage($"Error in UpdatePathImage of ProfilesInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Removes an image associated with the client's profile.
        /// </summary>
        /// <param name="client">The client whose image is to be removed.</param>
        /// <param name="imageName">The name of the image to remove.</param>
        /// <returns>Returns the removed image if successful, otherwise null.</returns>
        public async Task<HubImage?> RemoveImage(Client client, string imageName)
        {
            try
            {
                var data = await _context.ClientData.FirstOrDefaultAsync(cd => cd.ClientDataId == client.ClientDataId);

                var img = data!.Image;

                _context.Images.Remove(img);
                await _context.SaveChangesAsync();

                return img;
            }
            catch (Exception ex)
            {
                printMessage($"Error in RemoveImage of ProfilesInfo: {ex.Message}");
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
