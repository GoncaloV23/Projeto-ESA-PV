using EasyFitHub.Models.Account;
using EasyFitHub.Models.Gym;
using EasyFitHub.Models.Profile;
using EasyFitHub.Models.Statistics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Packaging;
using System.Runtime.Intrinsics.X86;

namespace EasyFitHub.Data
{
    /// <summary>
    /// AUTHOR: Francisco Silva
    /// Usado pra efetuar operações CRUD de estatísticas no contexto
    /// </summary>
    public class StatisticsInfo
    {
        private readonly EasyFitHubContext _context;
        private readonly ILogger? _logger;
        public StatisticsInfo(EasyFitHubContext context, ILogger? logger = null)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Creates statistics for a specific gym.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        public async Task CreateGymStats(int gymId)
        {
            try { 
                var gym = await _context.Gym
                    .Include(g => g.Items)
                    .Include(g => g.Machines)
                    .Include(g => g.GymClients).ThenInclude(gc => gc.Client).ThenInclude(c => c.User)
                    .Include(g => g.GymEmployees)
                    .SingleOrDefaultAsync(g => g.Id == gymId);
                if (gym == null) return;

                var clientCount = gym.GymClients.Count();
                var ptCount = gym.GymEmployees.Where(e => e.Role == Role.PT).Count();
                var secretaryCount = gym.GymEmployees.Where(e => e.Role == Role.SECRETARY).Count();
                var nutricionistCount = gym.GymEmployees.Where(e => e.Role == Role.NUTRICIONIST).Count();

                var gymStats = new GymStats
                {
                    ClientCount = clientCount,
                    PTCount = ptCount,
                    NutricionistCount = nutricionistCount,
                    SecretaryCount = secretaryCount,
                    ShopItemCount = gym.Items.Count,
                    MachineCount = gym.Machines.Count,
                    Gym = gym
                };

                var clients = gym.GymClients.Select(gc => gc.Client);

                var userCount = clients.Count();
                double mascCount = clients.Where(c => c.Gender == Gender.MASCULINE).Count();
                double femCount = clients.Where(c => c.Gender == Gender.FEMININE).Count();
                Dictionary<string, double> sexRatesDic = new Dictionary<string, double>();
                if (userCount == 0)
                {
                    sexRatesDic.AddRange(new[]
                    {
                        new KeyValuePair<string, double>("Masculine", 0),
                        new KeyValuePair<string, double>("Feminine", 0),
                        new KeyValuePair<string, double>("Undefined", 0)
                    });
                }
                else
                {
                    sexRatesDic.AddRange(new[]
                    {
                        new KeyValuePair<string, double>("Masculine", mascCount / userCount),
                        new KeyValuePair<string, double>("Feminine", femCount / userCount),
                        new KeyValuePair<string, double>("Undefined", (userCount - mascCount - femCount) / userCount)
                    });
                }
                gymStats.SetSexRatesDictionary(sexRatesDic);


                var ageRatesDic = new Dictionary<int, double> {
                    {0,0 },
                    {18,0 },
                    {30,0 },
                    {50,0 },
                };
                
                if (userCount > 0)
                {
                    var ages = clients.Select(c => c.User.BirthDate).ToList();
                    ages.ForEach(birth =>
                    {
                        int age = DateTime.Today.Year - birth.Year;
                        if (DateTime.Today.Month < birth.Month || (DateTime.Today.Month == birth.Month && DateTime.Today.Day < birth.Day))
                        {
                            age--;
                        }
                        if (age < 18) ageRatesDic[0] += 1;
                        else if (age < 30) ageRatesDic[18] += 1;
                        else if (age < 50) ageRatesDic[30] += 1;
                        else ageRatesDic[50] += 1;
                    });
                    foreach (var entry in ageRatesDic)
                    {
                        ageRatesDic[entry.Key] = entry.Value / userCount;
                    }
                }
                gymStats.SetAgeRatesDictionary(ageRatesDic);

                await _context.GymStats.AddAsync(gymStats);

                await _context.SaveChangesAsync();
            }catch (Exception ex)
            {
                printMessage($"Error in CreateGymStats of StatisticsInfo: {ex.Message}");
            }
        }
        /// <summary>
        /// Retrieves a list of clients associated with a specific employee.
        /// </summary>
        /// <param name="clientId">The ID of the employee.</param>
        /// <returns>Returns a list of clients.</returns>
        private async Task<List<Client>> GetClients(int clientId)
        {
            try { 
                var gyms = _context.Gym
                    .Where(g => g.GymEmployees.Any(gc => gc.ClientId == clientId));

                List<Client> clients = await gyms
                    .SelectMany(g => g.GymClients.Where(ge => ge.GymEmployees.Any(c => c.GymEmployee.ClientId == clientId)))
                    .Distinct()
                    .Select(ge => ge.Client)
                    .ToListAsync();

                return clients;
            }catch (Exception ex)
            {
                printMessage($"Error in GetClients of StatisticsInfo: {ex.Message}");
                return new List<Client>();
            }
        }
        /// <summary>
        /// Creates statistics for a specific employee.
        /// </summary>
        /// <param name="clientId">The ID of the employee.</param>
        public async Task CreateEmployeeStats(int clientId)
        {
            try { 
                var employee = await _context.Client.SingleOrDefaultAsync(c => c.ClientId == clientId);
                if (employee == null) return;

                var clients = await GetClients(clientId);

                var userCount = clients.Count();
                var employeeStats = new EmployeeStats
                {
                    ClientCount = userCount,
                    Client = employee
                };

                double mascCount = clients.Where(c => c.Gender == Gender.MASCULINE).Count();
                double femCount = clients.Where(c => c.Gender == Gender.FEMININE).Count();


                Dictionary<string, double> sexRatesDic = new Dictionary<string, double>();
                if (userCount == 0)
                {
                    sexRatesDic.AddRange(new[]
                    {
                        new KeyValuePair<string, double>("Masculine", 0),
                        new KeyValuePair<string, double>("Feminine", 0),
                        new KeyValuePair<string, double>("Undefined", 0)
                    });
                }
                else
                {
                    sexRatesDic.AddRange(new[]
                    {
                        new KeyValuePair<string, double>("Masculine", mascCount / userCount),
                        new KeyValuePair<string, double>("Feminine", femCount / userCount),
                        new KeyValuePair<string, double>("Undefined", (userCount - mascCount - femCount) / userCount)
                    });
                }
                employeeStats.SetUserSexRatesDictionary(sexRatesDic);


                var ageRatesDic = new Dictionary<int, double> {
                    {0,0 },
                    {18,0 },
                    {30,0 },
                    {50,0 },
                };
                if (userCount > 0)
                {
                    var ages = clients.Select(c => c.User.BirthDate).ToList();
                    ages.ForEach(birth =>
                    {
                        int age = DateTime.Today.Year - birth.Year;
                        if (DateTime.Today.Month < birth.Month || (DateTime.Today.Month == birth.Month && DateTime.Today.Day < birth.Day))
                        {
                            age--;
                        }
                        if (age < 18) ageRatesDic[0] += 1;
                        else if (age < 30) ageRatesDic[18] += 1;
                        else if (age < 50) ageRatesDic[30] += 1;
                        else ageRatesDic[50] += 1;
                    });
                    foreach (var entry in ageRatesDic)
                    {
                        ageRatesDic[entry.Key] = entry.Value / userCount;
                    }
                }
                employeeStats.SetUserAgeRatesDictionary(ageRatesDic);

                await _context.EmployeeStats.AddAsync(employeeStats);

                await _context.SaveChangesAsync();
            }catch (Exception ex)
            {
                printMessage($"Error in CreateEmployeeStats of StatisticsInfo: {ex.Message}");
            }
        }
        /// <summary>
        /// Creates platform-wide statistics.
        /// </summary>
        public async Task CreatePlatformStats()
        {
            try { 
                var gymCount = await _context.Gym.CountAsync(g => g.IsConfirmed);
                var userCount = await _context.Client.CountAsync();
                var ages = await _context.Client.Select(c => c.User.BirthDate).ToListAsync();
                double avg = 0;

                ages.ForEach(birth =>
                {
                    int age = DateTime.Today.Year - birth.Year;
                    if (DateTime.Today.Month < birth.Month || (DateTime.Today.Month == birth.Month && DateTime.Today.Day < birth.Day))
                    {
                        age--;
                    }

                    avg += age;
                });
                avg = avg / ages.Count();

                var platformStats = new PlatformStats
                {
                    GymCount = gymCount,
                    UserCount = userCount,
                    AvgAge = avg
                };

                double femCount = await _context.Client.CountAsync(g => g.Gender == Gender.FEMININE);
                double mascCount = await _context.Client.CountAsync(g => g.Gender == Gender.MASCULINE);

                Dictionary<string, double> sexRatesDic = new Dictionary<string, double>();
                if (userCount == 0) 
                {
                    sexRatesDic.AddRange(new[]
                    {
                        new KeyValuePair<string, double>("Masculine", 0),
                        new KeyValuePair<string, double>("Feminine", 0),
                        new KeyValuePair<string, double>("Undefined", 0)
                    });
                } 
                else 
                {
                    sexRatesDic.AddRange(new[]
                    {
                        new KeyValuePair<string, double>("Masculine", mascCount / userCount),
                        new KeyValuePair<string, double>("Feminine", femCount / userCount),
                        new KeyValuePair<string, double>("Undefined", (userCount - mascCount - femCount) / userCount)
                    });
                }
                
                platformStats.SetSexRatesDictionary(sexRatesDic);

                var topGyms = await _context.Gym
                    .Include(g => g.GymClients)
                    .Where(g => g.IsConfirmed)
                    .OrderByDescending(g => g.GymClients.Count)
                    .Take(10)
                    .ToListAsync();
                var topGymsDic = new Dictionary<string, int>();
                topGyms.ForEach(gym =>
                {
                    topGymsDic[gym.Name] = gym.GymClients.Count;
                });
                platformStats.SetTopGymsDictionary(topGymsDic);

                await _context.PlatformStats.AddAsync(
                    platformStats
                );

                await _context.SaveChangesAsync();
            }catch (Exception ex)
            {
                printMessage($"Error in CreatePlatformStats of StatisticsInfo: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates statistics for a specific client.
        /// </summary>
        /// <param name="clientId">The ID of the client.</param>
        public async Task CreateClientStats(int clientId)
        {
            try { 
                var client = await _context.Client.Include(c => c.Biometrics).SingleOrDefaultAsync(c => c.ClientId == clientId);

                if (client == null) return;

                var clientStats = new ClientStats
                {
                    Weigth = client.Biometrics.Weigth,
                    Height = client.Biometrics.Height,
                    FatMass = client.Biometrics.FatMass,
                    LeanMass = client.Biometrics.LeanMass,
                    BodyMassIndex = client.Biometrics.BodyMassIndex,
                    VisceralFat = client.Biometrics.VisceralFat,
                    Client = client,
                };

                await _context.ClientStats.AddAsync(clientStats);
                await _context.SaveChangesAsync();
            }catch (Exception ex)
            {
                printMessage($"Error in CreateClientStats of StatisticsInfo: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves statistics for employees associated with a specific client.
        /// </summary>
        /// <param name="clientId">The ID of the client.</param>
        /// <returns>Returns a list of employee statistics.</returns>
        public async Task<List<EmployeeStats>> GetEmployeeStats(int clientId)
        {
            try { 
                return await _context.EmployeeStats
                    .Include(s => s.UserSexRates)
                    .Include(s => s.UserAgeRates)
                    .Include(s => s.Client).ThenInclude(c => c.User)
                    .Where(cs => cs.ClientId == clientId).ToListAsync();
            }catch (Exception ex)
            {
                printMessage($"Error in GetEmployeeStats of StatisticsInfo: {ex.Message}");
                return new List<EmployeeStats>();
            }
        }

        /// <summary>
        /// Retrieves statistics for a specific client.
        /// </summary>
        /// <param name="clientId">The ID of the client.</param>
        /// <returns>Returns a list of client statistics.</returns>
        public async Task<List<ClientStats>> GetClientStats(int clientId)
        {
            try
            {
                return await _context.ClientStats
                    .Include(s => s.Client).ThenInclude(c => c.User)
                    .Where(cs => cs.ClientId == clientId).ToListAsync();
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetClientStats of StatisticsInfo: {ex.Message}");
                return new List<ClientStats>();
            }
        }

        /// <summary>
        /// Retrieves statistics for a specific gym.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <returns>Returns a list of gym statistics.</returns>
        public async Task<List<GymStats>> GetGymStats(int gymId)
        {
            try 
            { 
                return await _context.GymStats
                    .Include(s => s.AgeRates)
                    .Include(s => s.SexRates)
                    .Include(s => s.Gym)
                    .Where(g => g.GymId == gymId).ToListAsync();
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetGymStats of StatisticsInfo: {ex.Message}");
                return new List<GymStats>();
            }
        }

        /// <summary>
        /// Retrieves platform-wide statistics.
        /// </summary>
        /// <returns>Returns a list of platform-wide statistics.</returns>
        public async Task<List<PlatformStats>> GetPlatformStats()
        {
            try 
            { 
                return await _context.PlatformStats
                    .Include(s => s.SexRates)
                    .Include(s => s.TopGyms)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetPlatformStats of StatisticsInfo: {ex.Message}");
                return new List<PlatformStats>();
            }
        }

        /// <summary>
        /// Creates various statistics.
        /// </summary>
        public async Task CreateStats()
        {
            try
            {
                await CreatePlatformStats();

                var gymsIds = await _context.Gym.Where(g => g.IsConfirmed).Select(g => g.Id).ToListAsync();
                foreach (var id in gymsIds)
                {
                    await CreateGymStats(id);
                }

                var employeesIds = await _context.GymEmployees.Select(e => e.ClientId).ToListAsync();
                foreach (var id in employeesIds)
                {
                    await CreateEmployeeStats(id);
                }

                var clientsIds = await _context.GymClients.Select(c => c.ClientId).ToListAsync();
                foreach (var id in clientsIds)
                {
                    await CreateClientStats(id);
                }
            }catch (Exception ex)
            {
                printMessage($"Error in CreateStats of StatisticsInfo: {ex.Message}");
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
