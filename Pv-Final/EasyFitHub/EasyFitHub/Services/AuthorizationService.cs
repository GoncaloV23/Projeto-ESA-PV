using EasyFitHub.Data;
using EasyFitHub.Models.Account;
using EasyFitHub.Models.Gym;

namespace EasyFitHub.Services
{
    /// <summary>
    /// AUTHOR: Rui Barroso
    /// Serviço usado na verificação the autorizações de um utilizador
    /// </summary>
    public class AuthorizationService
    {
        AuthenticationService _authenticationService;
        GymsInfo _gymsInfo;
        public AuthorizationService(AuthenticationService authenticationService, EasyFitHubContext context)
        {
            _authenticationService = authenticationService;
            _gymsInfo = new GymsInfo(context);
        }
        /// <summary>
        /// Classe Auxiliar usada para verificar as autorizaçoes de um utilizador
        /// </summary>
        public class AuthorizationResult
        {
            /// <summary>
            /// o ginasio
            /// </summary>
            public Gym? Gym { get; set; }
            /// <summary>
            /// A conta autenticada
            /// </summary>
            public Account? Account { get; set; }
            /// <summary>
            /// A conta pode ler detalhes do ginásio?
            /// </summary>
            public bool IsAuthorizedToRead { get; set; }
            /// <summary>
            /// A conta pode editar detalhes do ginasio?
            /// </summary>
            public bool IsAuthorizedToEdit { get; set; }
        }
        /// <summary>
        /// Verifica as autorizações de um utilizador num determinado ginasio
        /// </summary>
        /// <param name="gymId">Id do ginasio</param>
        /// <param name="requestSession">Session Token the autenticaçao</param>
        /// <returns>AuthorizationResult instance</returns>
        public async Task<AuthorizationResult> isAuthorized(int gymId, ISession requestSession)
        {
            var result = await GetGymAndAccount(gymId, requestSession);

            result = IsAuthorizedToRead(IsAuthorizedToEdit(result));

            return result;
        }

        /// <summary>
        /// retorna AuthorizationResult sem permissões de um ginásio e utilizador
        /// </summary>
        /// <param name="gymId">Id do ginásio</param>
        /// <param name="requestSession">Token de sessão de um Utilizador autenticado</param>
        /// <returns>AuthorizationResult sem permissões</returns>
        public async Task<AuthorizationResult> GetGymAndAccount(int gymId, ISession requestSession)
        {
            var account = await _authenticationService.GetAccount("token", requestSession.GetString("AccessToken"));
            var gym = await _gymsInfo.GetGym(gymId);

            return new AuthorizationResult
            {
                IsAuthorizedToEdit = false,
                IsAuthorizedToRead = false,
                Account = account,
                Gym = gym
            };
        }
        /// <summary>
        /// Devolve o Role de um User num Ginásio
        /// </summary>
        /// <param name="user">O utilizador</param>
        /// <param name="gym">O ginásio</param>
        /// <returns>O Role</returns>
        public Role? GetUserRole(User user, Gym gym)
        {
            if (IsEmployee(user, gym))
            {
                var employee = gym.GymEmployees.Where(e => e.Client.User.AccountId == user.AccountId).SingleOrDefault(); 
                return (employee != null) ? employee.Role : null;
            }
            else
            {
                var client = gym.GymClients.Where(client => client.Client.User.AccountId == user.AccountId).SingleOrDefault();
                return (client != null) ? client.Role : null;
            }


        }
        /// <summary>
        /// Verifica se um utilizador pode ler detalhes de um ginásio
        /// </summary>
        /// <param name="result">AuthorizationResult Instance com referencias ao utilizador e ao ginásio</param>
        /// <returns>AuthorizationResult com Permissões definidas</returns>
        public AuthorizationResult IsAuthorizedToRead(AuthorizationResult result)
        {
            Gym? gym = result.Gym;
            Account? account = result.Account;
            result.IsAuthorizedToRead = false;

            if (account == null || gym == null || !HasAutorization(account, gym))
                return result;

            result.IsAuthorizedToRead = true;

            return result;
        }        /// <summary>
                 /// Verifica se um utilizador pode editar detalhes de um ginásio
                 /// </summary>
                 /// <param name="result">AuthorizationResult Instance com referencias ao utilizador e ao ginásio</param>
                 /// <returns>AuthorizationResult com Permissões definidas</returns>
        public AuthorizationResult IsAuthorizedToEdit(AuthorizationResult result)
        {
            Gym? gym = result.Gym;
            Account? account = result.Account;
            result.IsAuthorizedToEdit = false;

            if (gym == null || account == null)
                return result;

            if (IsManager(account, gym) || IsAdmin(account, gym))
            {
                result.IsAuthorizedToEdit = true;
                return result;
            }

            if (account.AccountType == AccountType.USER)
                return result;

            var role = GetUserRole((User)account, gym);
            if (role != Role.SECRETARY)
                return result;

            result.IsAuthorizedToEdit = true;
            return result;

        }
        /// <summary>
        /// Verifica se um Account é employee de um Ginásio
        /// </summary>
        /// <param name="employee">A Account</param>
        /// <param name="gym">O ginásio</param>
        /// <returns>True se é vinculado; False caso contrário</returns>
        public bool IsEmployee(Account employee, Gym gym)
        {
            if (employee == null || gym == null || employee.AccountType != AccountType.USER ||
                !gym.GymEmployees.Any(gymEmployee => gymEmployee.Client != null && gymEmployee.Client.User.AccountId == employee.AccountId))
                return false;

            return true;
        }
        /// <summary>
        /// Verifica se um cliente pertence a um ginásio
        /// </summary>
        /// <param name="client">O Account do client</param>
        /// <param name="gym">O ginásio</param>
        /// <returns>True se é vinculado; False caso contrário</returns>
        public bool IsClient(Account client, Gym gym)
        {
            if (client == null || gym == null || client.AccountType != AccountType.USER ||
                !gym.GymClients.Any(gc => gc.Client != null && gc.Client.User.AccountId == client.AccountId))
                return false;

            return true;
        }
        /// <summary>
        /// Verifica se um Manager pertence a um ginásio
        /// </summary>
        /// <param name="manager">O Manager Account</param>
        /// <param name="gym">O ginásio</param>
        /// <returns>True se é vinculado; False caso contrário</returns>

        public bool IsManager(Account manager, Gym gym)
        {
            if (manager == null || gym == null || manager.AccountType != AccountType.MANAGER || ((Manager)manager).GymId != gym.Id)
                return false;

            return true;
        }
        /// <summary>
        /// Verifica se uma conta é Admin
        /// </summary>
        /// <param name="admin">A conta Admin</param>
        /// <param name="gym">Um ginásio qualquer</param>
        /// <returns>True se é Admin, False caso contrário</returns>
        public bool IsAdmin(Account admin, Gym gym)
        {
            if (admin == null || gym == null || admin.AccountType != AccountType.ADMIN)
                return false;

            return true;
        }
        /// <summary>
        /// Verifica se uma conta tem algum tipo de permissoes num ginásio
        /// </summary>
        /// <param name="account">A Account</param>
        /// <param name="gym">O ginásio</param>
        /// <returns>True se tem permissões, False caso contrário</returns>
        public bool HasAutorization(Account account, Gym gym)
        {
            switch (account.AccountType)
            {
                case AccountType.ADMIN:
                    return true;
                case AccountType.MANAGER:
                    return IsManager(account, gym);
                case AccountType.USER:
                    return
                        IsClient(account, gym) ||
                        IsEmployee(account, gym);
                default:
                    return false;

            }
        }
    }
}
