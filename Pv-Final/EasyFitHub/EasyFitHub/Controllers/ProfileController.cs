using EasyFitHub.Data;
using EasyFitHub.Models.Account;
using EasyFitHub.Models.Profile;
using EasyFitHub.Services;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

namespace EasyFitHub.Controllers
{

    /// <summary>
    /// Author: Rui Barroso
    /// Controlador usado para gerir operações relacionadas com Utilizadores e os seus Perfis
    /// </summary>
    public class ProfileController : Controller
    {
        private readonly ProfilesInfo _profilesInfo;
        private readonly AuthenticationService _authenticationService;
        private readonly FirebaseService _firebaseService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(EasyFitHubContext context, AuthenticationService authenticationService, FirebaseService firebaseService,ILogger<ProfileController> logger)
        {
            _profilesInfo = new ProfilesInfo(context, logger);
            _authenticationService = authenticationService;
            _firebaseService = firebaseService;
            _logger = logger;
        }
        /// <summary>
        /// Displays the profile of a user with the specified user ID.
        /// </summary>
        /// <param name="userId">The ID of the user whose profile is to be displayed.</param>
        /// <returns>Returns a view displaying the user's profile.</returns>
        public async Task<IActionResult> Index(int userId)
        {
            var sessionAcountt = await GetSessionAccount();

            Client client = _profilesInfo.GetUser(userId);

            if (client == null) return RedirectToAction("Index", "Home");


            return View(client);
        }
        /// <summary>
        /// Displays the profile editing page for a user with the specified user ID.
        /// </summary>
        /// <param name="userId">The ID of the user whose profile is to be edited.</param>
        /// <returns>Returns a view for editing the user's profile.</returns>
        public async Task<IActionResult> ProfileEdit(int userId)
        {
            var sessionAcountt = await GetSessionAccount();
            if (sessionAcountt == null) return RedirectToAction("Index", "Home");

            Client client = _profilesInfo.GetUser(userId);
            Client sessionClient = _profilesInfo.GetUser(sessionAcountt.AccountId);

            if (!IsSessionClient(sessionClient, client)) return RedirectToAction("Index", "Home");

            return View(client);
        }
        /// <summary>
        /// Displays the biometrics page for a user with the specified user ID.
        /// </summary>
        /// <param name="userId">The ID of the user whose biometrics page is to be displayed.</param>
        /// <returns>Returns a view displaying the user's biometrics.</returns>
        public async Task<IActionResult> Biometrics(int userId)
        {

            var sessionAcountt = await GetSessionAccount();
            if (sessionAcountt == null) return RedirectToAction("Index", "Home");

            Client client = _profilesInfo.GetUser(userId);
            Client sessionClient = _profilesInfo.GetUser(sessionAcountt.AccountId);

            if (!IsAutorizedToEdit(sessionClient, client)) return RedirectToAction("Index", "Home");

            return View(client);
        }
        /// <summary>
        /// Displays the biometrics editing page for a user with the specified user ID.
        /// </summary>
        /// <param name="userId">The ID of the user whose biometrics are to be edited.</param>
        /// <returns>Returns a view for editing the user's biometrics.</returns>
        public async Task<IActionResult> BiometricsEdit(int userId)
        {
            var sessionAcountt = await GetSessionAccount();
            if (sessionAcountt == null) return RedirectToAction("Index", "Home");

            Client client = _profilesInfo.GetUser(userId);
            Client sessionClient = _profilesInfo.GetUser(sessionAcountt.AccountId);

            if (!IsAutorizedToEdit(sessionClient, client)) return RedirectToAction("Index", "Home");

            return View(client);
        }

        /// <summary>
        /// Updates the client information with the provided data.
        /// </summary>
        /// <param name="newClient">The updated client data.</param>
        /// <returns>Returns a redirect to the user's profile page.</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateClient(Client newClient)
        {
            var sessionAcountt = await GetSessionAccount();
            if (sessionAcountt == null) return RedirectToAction("Index", "Home");

            Client? sessionClient = _profilesInfo.GetUser(sessionAcountt.AccountId);
            if (sessionClient == null) return RedirectToAction("Index", "Home");

            _logger.LogInformation("Aqui\n\n\n\n" + ((newClient == null) ?
                "Is NULL" :
                newClient.ToJson()
                + "\nClientId: "
                + newClient.ClientId
                + "\n\n\n\n\n"));

            if (!IsAutorizedToEdit(sessionClient, newClient)) return RedirectToAction("Index", "Home");

            var res = await _profilesInfo.UpdateClient(newClient.ClientId, newClient);

            return RedirectToAction("Index", new { userId = newClient.UserId });
        }




        /// <summary>
        /// Uploads a file for the user's profile picture.
        /// </summary>
        /// <param name="file">The file to upload.</param>
        /// <returns>Returns a redirect to the user's profile editing page.</returns>


        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var sessionAcountt = await GetSessionAccount();
                if (sessionAcountt == null) return RedirectToAction("Index", "Home");

                Client? sessionClient = _profilesInfo.GetUser(sessionAcountt.AccountId);
                if (sessionClient == null) return RedirectToAction("Index", "Home");



                var newHubImg = await _profilesInfo.CreateImage(sessionClient);

                using (var stream = file.OpenReadStream())
                {
                    newHubImg = await _firebaseService.CreateHubImage(newHubImg, stream, _logger);
                }


                await _profilesInfo.UpdatePathImage(sessionClient, newHubImg.Name, newHubImg.Path);
                return RedirectToAction("ProfileEdit", new { userId = sessionClient.UserId });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Deletes a file (image) associated with the user's profile.
        /// </summary>
        /// <param name="imageName">The name of the image to delete.</param>
        /// <returns>Returns a redirect to the user's profile editing page.</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteFile(string imageName)
        {
            var sessionAcountt = await GetSessionAccount();
            if (sessionAcountt == null) return RedirectToAction("Index", "Home");

            Client? sessionClient = _profilesInfo.GetUser(sessionAcountt.AccountId);
            if (sessionClient == null) return RedirectToAction("Index", "Home");

            var image = await _profilesInfo.RemoveImage(sessionClient, imageName);

            var res = await _firebaseService.DeleteHubImage(image!);

            return RedirectToAction("ProfileEdit", new { userId = sessionClient.UserId });
        }

        /// <summary>
        /// Retrieves the session account based on the provided access token.
        /// </summary>
        /// <returns>Returns the session account if authenticated, otherwise null.</returns>
        private async Task<Account?> GetSessionAccount()
        {
            return await _authenticationService.GetAccount("token", HttpContext.Session.GetString("AccessToken"));
        }

        /// <summary>
        /// Checks if the session client is authorized to edit the specified user's profile.
        /// </summary>
        /// <param name="sessionClient">The session client.</param>
        /// <param name="user">The user whose profile is to be edited.</param>
        /// <returns>Returns true if authorized, otherwise false.</returns>
        private bool IsAutorizedToEdit(Client sessionClient, Client user)
        {
            if (sessionClient == null || user == null) return false;

            if (IsSessionClient(sessionClient, user)) return true;

            return IsClientOf(sessionClient, user);
        }
        /// <summary>
        /// Checks if the session client matches the specified user.
        /// </summary>
        /// <param name="sessionClient">The session client.</param>
        /// <param name="user">The user to compare with.</param>
        /// <returns>Returns true if the session client matches the user, otherwise false.</returns>
        private bool IsSessionClient(Client sessionClient, Client user)
        {
            return user.UserId == sessionClient.UserId;
        }
        /// <summary>
        /// Checks if the session client is an employee of the specified user.
        /// </summary>
        /// <param name="employee">The session client.</param>
        /// <param name="user">The user to check.</param>
        /// <returns>Returns true if the session client is an employee of the user, otherwise false.</returns>
        private bool IsClientOf(Client employee, Client user)
        {
            return _profilesInfo.GetEmployees(user).Any(e => e.ClientId == employee.ClientId);
        }
    }
}
