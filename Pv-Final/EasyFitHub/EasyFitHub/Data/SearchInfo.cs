using EasyFitHub.Models.Account;
using EasyFitHub.Models.Gym;
using EasyFitHub.Models.Profile;
using Microsoft.EntityFrameworkCore;

namespace EasyFitHub.Data
{
    /// <summary>
    /// AUTHOR: Rui Barroso
    /// Destinado a efetuar querys aos ginasios
    /// </summary>
    public class SearchInfo
    {
        private readonly EasyFitHubContext _context;
        private readonly ILogger? _logger;

        /// <summary>
        /// Construtor da classe SearchInfo
        /// </summary>
        /// <param name="context">Contexto do banco de dados</param>
        /// <param name="logger">Logger para registrar mensagens de erro</param>
        public SearchInfo(EasyFitHubContext context, ILogger? logger = null)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtém uma lista de ginasios confirmados
        /// </summary>
        /// <returns>Lista de ginasios</returns>
        public async Task<List<Gym>> GetGyms()
        {
            try
            {
                return await _context.Gym
                    .Where(g => g.IsConfirmed)
                    .Include(g => g.Images)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetGymsBySubscription: {ex.Message}");
                return new List<Gym>();
            }
        }

        /// <summary>
        /// Obtém uma lista de ginasios por localização
        /// </summary>
        /// <param name="location">Localização a ser pesquisada</param>
        /// <returns>Lista de ginasios</returns>
        public async Task<List<Gym>> GetGymsByLocation(string location)
        {
            try
            {
                return await _context.Gym
                    .Where(g => g.IsConfirmed && g.Location.Trim().ToLower().Contains(location.Trim().ToLower()))
                    .Include(g => g.Images)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetGymsBySubscription: {ex.Message}");
                return new List<Gym>();
            }
        }

        /// <summary>
        /// Obtém uma lista de ginasios por nome
        /// </summary>
        /// <param name="name">Nome do ginasio a ser pesquisado</param>
        /// <returns>Lista de ginasios</returns>
        public async Task<List<Gym>> GetGymsByName(string name)
        {
            try
            {
                return await _context.Gym
                    .Where(g => g.IsConfirmed && g.Name.Trim().ToLower().Contains(name.Trim().ToLower()))
                    .Include(g => g.Images)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetGymsBySubscription: {ex.Message}");
                return new List<Gym>();
            }
        }

        /// <summary>
        /// Obtém uma lista de ginasios associados à conta do cliente
        /// </summary>
        /// <param name="account">Conta do cliente</param>
        /// <returns>Lista de ginasios</returns>
        public async Task<List<Gym>> GetGymsBySubscription(Account account)
        {
            try
            {
                Client? client = await _context.Client.SingleOrDefaultAsync(c => c.UserId == account.AccountId);
                if (client == null) return new List<Gym>();

                return await _context.Gym
                    .Where(g => g.GymClients.Any(gc => gc.ClientId == client.ClientId) || g.GymEmployees.Any(gc => gc.ClientId == client.ClientId))
                    .Include(g => g.Images)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetGymsBySubscription: {ex.Message}");
                return new List<Gym>();
            }
        }

        /// <summary>
        /// Método privado para imprimir mensagens de erro
        /// </summary>
        /// <param name="message">Mensagem de erro</param>
        private void printMessage(string message)
        {
            if (_logger == null)
                Console.WriteLine($"\n\n\n\n{message}\n\n\n\n");
            else
                _logger.LogError($"\n\n\n\n{message}\n\n\n\n");
        }
    }
}