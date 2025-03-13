using EasyFitHub.Models.Account;
using EasyFitHub.Models.Gym;
using EasyFitHub.Utils;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Common;
using System.Reflection.Metadata.Ecma335;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace EasyFitHub.Data
{
    public class AuthenticationsInfo
    {
        /// <summary>
        /// AUTHOR:André P.
        /// classe que fornece querys relativas ao modulo de autenticaçoes
        /// todos os metodos retornam objetos do tipo IQueryable<> pra manter consistencia
        /// </summary>


        private readonly EasyFitHubContext _context;
        private readonly ILogger? _logger;
        public AuthenticationsInfo(EasyFitHubContext context, ILogger? logger = null)
        {
            _context = context;
            _logger = logger;
        }
        /// <summary>
        /// Devolve uma conta consoate o seu email e password
        /// </summary>
        /// <param name="password"> a password da conta </param>
        /// <param name="userName">o Nome unico da conta</param>
        /// <returns>a conta associada ao email e pw</returns>
        public IQueryable<Account>? LoginAccount(string password, string userName)
        {
            try 
            {
                var accEntry = _context.Account.Where(t => t.UserName == userName);
                            
                if (accEntry != null && accEntry.FirstOrDefault()  != null && PasswordHasher.VerifyPassword(password, accEntry.FirstOrDefault().Password))
                {

                    printMessage($"{password}\n{PasswordHasher.HashPassword(password)}\n{accEntry.FirstOrDefault().Password}");

                    return accEntry;
                }
                return null;
            }
            catch (Exception ex)
            {
                printMessage($"Error in LoginAccount of AuthenticationsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// regista uma conta na BD
        /// </summary>
        /// <param name="account">a conta a ser adicionada</param>
        /// <returns>query para a nova conta, se a operaçao suceder</returns>
        public IQueryable<Account>? RegisterAccount(Account account)
        {
            try 
            {
                var theQuery = _context.Account.Where(a => a.AccountId == account.AccountId || a.Email == account.Email || a.UserName == account.UserName);
                if (theQuery.Any()) return null;

                account.Password = PasswordHasher.HashPassword(account.Password);
                _context.Account.Add(account);
                _context.SaveChanges();
                var theNewAccount = _context.Account.Where(a => a.Email== account.Email);
                
                if (theNewAccount.Any()) return theNewAccount;

                return null;
            }
            catch (Exception ex)
            {
                printMessage($"Error in RegisterAccount of AuthenticationsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// retorna a password associada a uma conta
        /// </summary>
        /// <param name="account">a conta em si</param>
        /// <returns>query para a password de account</returns>
        public IQueryable<string>? GetPassword(Account account)
        {
            try 
            { 
                var theQuery = _context.Account
                            .Where(a => a.AccountId == account.AccountId)
                            .Select(p => p.Password);
                if (theQuery.Any()) return theQuery;
                return null;
            }
            catch (Exception ex)
            {
                printMessage($"Error in CheckRecoveryCode of AuthenticationsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// altera a password de uma conta
        /// </summary>
        /// <param name="account">a conta em sim</param>
        /// <param name="oldPassword">a password antiga, usada na verificaçao</param>
        /// <param name="newpassword">a nova password</param>
        /// <returns>query para a conta com a password alterada na BD</returns>
        public IQueryable<Account>? ChangePassword(Account account, string oldPassword, string newpassword)
        {
            try 
            { 
                var accEntry = _context.Account.Where(t => t.AccountId == account.AccountId);

                if (accEntry.Any() && PasswordHasher.VerifyPassword(oldPassword, accEntry.FirstOrDefault().Password))
                {
                    accEntry.FirstOrDefault().Password = PasswordHasher.HashPassword(newpassword);
                    _context.SaveChanges();
                    return _context.Account.Where(t => t.AccountId == account.AccountId);
                }
                return null;
            }
            catch (Exception ex)
            {
                printMessage($"Error in ChangePassword of AuthenticationsInfo: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// verifica se 'account' existe
        /// </summary>
        /// <param name="account">a conta a ser verificada</param>
        /// <returns>query para a conta, caso exista da BD</returns>
        public IQueryable<Account>? CheckCredentials(Account account)
        {
            try 
            { 
                var theQuery = _context.Account
                            .Where(b => b.AccountId == account.AccountId &&
                                            b.UserName == account.UserName &&
                                            b.Password == account.Password &&
                                            b.AccountType == account.AccountType);
                    if (theQuery.Any()) return theQuery;
                    return null;
            }
            catch (Exception ex)
            {
                printMessage($"Error in CheckCredentials of AuthenticationsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// verifica se uma conta encontre autenticada ou 'logged in'
        /// </summary>
        /// <param name="token">identificador de uma autenticação</param>
        /// <returns>a bool, true se estirver autenticada e false caso contrario.</returns>
        public async Task<bool> IsLoged(string token)
        {
            try 
            { 
                if(token == null) {  return false; }
                var account = await GetAccountWithToken(token);

                return account != null;

            }
            catch (Exception ex)
            {
                printMessage($"Error in IsLoged of AuthenticationsInfo: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// retorna uma conta atraves do seu id
        /// </summary>
        /// <param name="id">o id da conta a ser devoilveida</param>
        /// <returns>a query para a conta do id fornecido caso esta exista na BD</returns>
        public async Task<Account?> GetAccount(int id)
        {
            try 
            { 
                return await _context.Account.SingleOrDefaultAsync(a => a.AccountId == id);
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetAccount of AuthenticationsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// retorna uma conta atraves do seu userName
        /// </summary>
        /// <param name="userName">o nome unico da conta a ser devoilveida</param>
        /// <returns>a query para a conta do nome fornecido caso esta exista na BD</returns>
        public Task<Account?> GetAccount(string userName)
        {
            try 
            { 
                return _context.Account.SingleOrDefaultAsync(a => a.UserName == userName);
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetAccount of AuthenticationsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// retorna uma conta atraves do seu token
        /// </summary>
        /// <param name="token">um token</param>
        /// <returns>a query para a conta do nome fornecido caso esta exista na BD</returns>
        public Task<Account?> GetAccountWithToken(string token)
        {
            try 
            { 
                if (token == null) return Task.FromResult<Account?>(null);

                return _context.Account.SingleOrDefaultAsync(a => a.Token == token);
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetAccountWithToken of AuthenticationsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// atualiza o token de uma account
        /// </summary>
        /// <param name="account">um instancia de Account</param>
        /// <param name="token">um token</param>
        /// <returns>a query para a conta do nome fornecido caso esta exista na BD</returns>
        public void UpdateAccountToken(Account account, string token) 
        {
            try 
            { 
                var accEntry = _context.Account
                            .Where(t => t.AccountId == account.AccountId);

                if (accEntry.Any())
                {
                    accEntry.FirstOrDefault()!.Token = token;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                printMessage($"Error in UpdateAccountToken of AuthenticationsInfo: {ex.Message}");
            }
        }

        public async Task<User?> CreateUser(User newUser)
        {
            try 
            { 
                var query = RegisterAccount(newUser);
                if (query == null) return null;
                
                var account = await query.SingleAsync(c => c.AccountId == newUser.AccountId);
                if (account == null) return null;

                var user = (User)account;

                var client = new Models.Profile.Client 
                {
                    User = user, 
                    Description = "No Description!", 
                    Gender = Models.Profile.Gender.UNDEFINED,
                    Biometrics = new Models.Profile.Biometrics(),
                    Data = new Models.Profile.ClientData(),
                };

                await _context.Client.AddAsync(client);

                await _context.SaveChangesAsync();

                return user;

            }
            catch (Exception ex)
            {
                printMessage($"Error in CreateUser of AuthenticationsInfo: {ex.Message}");
                return null;
            }
        }




        public async Task<string?> GenerateRecoveryCode(int accountId)
        {
            try 
            { 
                var account = await _context.Account.SingleOrDefaultAsync(a => a.AccountId == accountId);
                if (account == null) return null;

                Random random = new Random();
                string generatedCode = random.Next(10000, 99999).ToString();

                account.RecoverCode = generatedCode;

                await _context.SaveChangesAsync();

                return generatedCode;
            }
            catch (Exception ex)
            {
                printMessage($"Error in GenerateRecoveryCode of AuthenticationsInfo: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> ClearRecoveryCode(int accountId)
        {
            try 
            { 
                var account = await _context.Account.SingleOrDefaultAsync(a => a.AccountId == accountId);
                if (account == null) return null;

                string? recoverCode = account.RecoverCode;
                account.RecoverCode = null;

                await _context.SaveChangesAsync();

                return recoverCode;
            }
            catch (Exception ex)
            {
                printMessage($"Error in ClearRecoveryCode of AuthenticationsInfo: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CheckRecoveryCode(int accountId, string code)
        {
            try 
            { 
                var account = await _context.Account.SingleOrDefaultAsync(a => a.AccountId == accountId);
                if (account == null) return false;

                return account.RecoverCode == code;
            }
            catch (Exception ex)
            {
                printMessage($"Error in CheckRecoveryCode of AuthenticationsInfo: {ex.Message}");
                return false;
            }

        }


        private void printMessage(string message)
        {
            if (_logger == null)
                Console.WriteLine($"\n\n\n\n{message}\n\n\n\n");
            else
                _logger.LogError($"\n\n\n\n{message}\n\n\n\n");
        }

        public async Task<List<Account>> GetAccounts()
        {
            return await _context.Account.ToListAsync();
        }
    }
}

