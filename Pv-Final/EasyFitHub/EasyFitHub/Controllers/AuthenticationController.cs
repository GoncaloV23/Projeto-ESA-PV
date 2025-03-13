using EasyFitHub.Data;
using EasyFitHub.Models.Account;
using EasyFitHub.Services;
using EasyFitHub.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace EasyFitHub.Controllers
{
    /// <summary>
    /// Author: Rui Barroso
    /// Controller responsible for handling authentication-related operations.
    /// </summary>
    public class AuthenticationController : Controller
    {
        private AuthenticationsInfo _authenticationInfo;
        private const string _tokenPrivateKey = "Chave Privada Suficientemente Longa";
        private readonly ILogger<AuthenticationController> _logger;
        private ValidationService _validationService;

        public AuthenticationController(EasyFitHubContext dbContext, ILogger<AuthenticationController> logger, ValidationService validationService)
        {
            _authenticationInfo  = new AuthenticationsInfo(dbContext, logger);
            _logger = logger;
            _validationService = validationService;
        }


        /// <summary>
        /// Displays the login page.
        /// </summary>
        /// <returns>Returns the login view.</returns>
        public async Task<ActionResult> Index()
        {
            string? sessionToken = HttpContext.Session.GetString("AccessToken");

            if (sessionToken != null && await _authenticationInfo.IsLoged(sessionToken)) 
                return RedirectToAction("Index", "Home");

            return View();
        }
        /// <summary>
        /// Displays the registration page based on the provided account type.
        /// </summary>
        /// <param name="accountType">The type of account to register.</param>
        /// <returns>Returns the registration view.</returns>
        // GET: Authentication/Signup?accountType=user
        // GET: Authentication/Signup?accountType=manager
        [Route("Authentication/Signup")]
        public ActionResult Registration(string accountType)
        {
            if (Enum.TryParse(accountType, true, out AccountType parsedAccountType))
            {
                var model = new Account();
                switch (parsedAccountType)
                {
                    case AccountType.USER:
                        return View();
                    case AccountType.MANAGER:
                        return View("GymRegistration");
                    default:
                        return View();
                }
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// Displays the account recovery page.
        /// </summary>
        /// <returns>Returns the account recovery view.</returns>
        // GET: Authentication/RecoverAccount
        [Route("Authentication/RecoverAccount")]
        public ActionResult AccountRecover()
        {
            return View();
        }

        /// <summary>
        /// Handles the login request.
        /// </summary>
        /// <param name="userName">The username of the account.</param>
        /// <param name="password">The password of the account.</param>
        /// <returns>Returns the appropriate view based on login status.</returns>
        [HttpPost]
        public ActionResult Login(string userName, string password) 
        {
            if (!_validationService.ValidateString(userName, 20, 4) ||
                !_validationService.ValidateString(password, 20, 4)) 
            {
                TempData["ResultMessage"] = "The credentials did not folow the required restrictions!";
                return View("Index");
            }


            var query = _authenticationInfo.LoginAccount(password, userName);
            var account = (query == null) ? null : query.FirstOrDefault<Account>();

            if (account != null)
            {
                
                string token = TokenGenerator.GenerateToken(_tokenPrivateKey, account.UserName);

                
                _authenticationInfo.UpdateAccountToken(account, token);

                
                HttpContext.Session.SetString("AccessToken", token);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ResultMessage"] = "The account was not loged!";
                return View("Index");
            }
        }/// <summary>
/// Handles the logout request.
/// </summary>
/// <returns>Returns the home view after logout.</returns>
        public async Task<ActionResult> Logout()
        {
            string? sessionToken = HttpContext.Session.GetString("AccessToken");

            var account = await _authenticationInfo.GetAccountWithToken(sessionToken);

            if (account != null)
            {
                _authenticationInfo.UpdateAccountToken(account, null);
            }
            

            return RedirectToAction("Index", "Home");
        }
        /// <summary>
        /// Handles the user registration request.
        /// </summary>
        /// <param name="newAccount">The user account information to be registered.</param>
        /// <returns>Returns the appropriate view based on registration status.</returns>

        [HttpPost]
        public async Task<ActionResult> RegisterUser(User newAccount)
        {
            var user = await _authenticationInfo.CreateUser(newAccount);


            if (user != null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ResultMessage"] = "The account was not added!";
                return View("Registration");
            }
        }
        /// <summary>
        /// Handles the gym manager registration request.
        /// </summary>
        /// <param name="newAccount">The manager account information to be registered.</param>
        /// <returns>Returns the appropriate view based on registration status.</returns>
        [HttpPost]
        public ActionResult RegisterGym(Manager newAccount)
        {
            var query = _authenticationInfo.RegisterAccount(newAccount);
            var addedAccount = (query == null) ? null : query.FirstOrDefault<Account>();


            if (addedAccount != null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ResultMessage"] = "The account was not added!";
                return View("Registration", new { accountType = AccountType.MANAGER });
            }
        }
        /// <summary>
        /// Handles the password recovery request.
        /// </summary>
        /// <param name="userName">The username of the account.</param>
        /// <param name="email">The email address associated with the account.</param>
        /// <returns>Returns the appropriate view based on password recovery status.</returns>
        [HttpPost]
        public async Task<ActionResult> RecoverPassword(string userName, string email) 
        {
            if (!_validationService.ValidateString(userName, 20, 4) ||
                !_validationService.ValidateEmail(email, 50, 5))
            {
                TempData["ResultMessage"] = "The credentials did not folow the required restrictions!";
                return View("Index");
            }

            var account = await _authenticationInfo.GetAccount(userName);

            if (account != null && account.Email == email)
            {
                // Gerar e enviar e-mail de recuperação de senha
                var code = await _authenticationInfo.GenerateRecoveryCode(account.AccountId);

                EmailSender.SendPasswordRecoveryEmail(account, code!);


                TempData["ResultMessage"] = "Your Account Information was send to your email!";
                return RedirectToAction("ChangePassword");
            }
            else
            {
                TempData["ResultMessage"] = "Your User Name and Email do not match!";
                return RedirectToAction("Index");
            }
        }
        /// <summary>
        /// Displays the change password page.
        /// </summary>
        /// <param name="userId">The ID of the user whose password is to be changed.</param>
        /// <returns>Returns the change password view.</returns>
        public async Task<ActionResult> ChangePassword(int userId = 0)
        {

            var account = await _authenticationInfo.GetAccount(userId);
            if (account == null) 
            { 
                return View("ForgotPassword"); 
            }

            ViewData["UserId"] = account.AccountId;
            return View("ChangePassword");
        }
        /// <summary>
        /// Handles the password change request.
        /// </summary>
        /// <param name="userId">The ID of the user whose password is to be changed.</param>
        /// <param name="newPassword">The new password.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <returns>Returns the appropriate view based on password change status.</returns>
        [HttpPost]
        public async Task<ActionResult> ChangePassword(int userId, string newPassword, string oldPassword) 
        {
            if (!_validationService.ValidateString(newPassword, 20, 4) ||
                !_validationService.ValidateString(oldPassword, 20, 4))
            {
                TempData["ResultMessage"] = "The credentials did not folow the required restrictions!";
                return RedirectToAction("ChangePassword", new { userId = userId });
            }

            var account = await _authenticationInfo.GetAccount(userId);
            var result = 
                (account == null)? 
                    null : 
                    _authenticationInfo.ChangePassword(account, oldPassword, newPassword);

            if (result != null)
            {
                TempData["ResultMessage"] = "Your Password was updated!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ResultMessage"] = "Your Password was is Incorrect!";
                return RedirectToAction("ChangePassword", new {userId = userId});
            }
        }
        /// <summary>
        /// Handles the password change request using a recovery code.
        /// </summary>
        /// <param name="userName">The username of the account.</param>
        /// <param name="code">The recovery code.</param>
        /// <param name="password">The new password.</param>
        /// <param name="confirmPassword">The confirmation of the new password.</param>
        /// <returns>Returns the appropriate view based on password change status.</returns>
        [HttpPost]
        public async Task<ActionResult> ChangePasswordWithCode( string userName, string code, string password, string confirmPassword)
        {
            if (
                !_validationService.ValidateString(userName, 20, 4) ||
                !_validationService.ValidateString(code, 5, 5) ||
                !_validationService.ValidateString(password, 20, 4) ||
                !_validationService.ValidateString(confirmPassword, 20, 4) 
            )
            {
                TempData["ResultMessage"] = "The credentials did not folow the required restrictions!";
                return RedirectToAction("ChangePassword");
            }

            if (password != confirmPassword)
            {
                TempData["ResultMessage"] = "Your Password and is not Equal to Confirm Field!";
                return RedirectToAction("ChangePassword");
            }

            var account = await _authenticationInfo.GetAccount(userName);
            if (account == null)
            {
                TempData["ResultMessage"] = "Your UserName was is Incorrect!";
                return RedirectToAction("ChangePassword");
            }

            var isCode = await _authenticationInfo.CheckRecoveryCode(account.AccountId, code);
            if (!isCode)
            {
                TempData["ResultMessage"] = "Your Code was is Incorrect!";
                return RedirectToAction("ChangePassword");
            }

            _authenticationInfo.ChangePassword(account, account.Password, password);
            await _authenticationInfo.ClearRecoveryCode(account.AccountId);

            TempData["ResultMessage"] = "Your Password was updated!";
            return RedirectToAction("Index");    
        }
    }
    /// <summary>
    /// Classe usada para a criação de Session Tokens para motivos de Autenticação
    /// </summary>
    public class TokenGenerator
    {
        /// <summary>
        /// É gerado um token para um usarname especifico
        /// </summary>
        /// <param name="secretKey">uma chave secreta usada pra gerar o Session Token</param>
        /// <param name="userName">O Username do utilizador em questão</param>
        /// <returns>O token gerado</returns>
        public static string GenerateToken(string secretKey, string userName)
        {
            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60), // Token expira em 60 minutos
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }

}


