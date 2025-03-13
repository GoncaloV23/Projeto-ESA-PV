using EasyFitHub.Data;
using EasyFitHub.Models;
using EasyFitHub.Models.Gym;
using EasyFitHub.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;

namespace EasyFitHub.Controllers
{
    /// <summary>
    /// Author: Rui Barroso
    /// Controlador usado para gerir as funcionalidades mais básicas do Sistema
    /// Usado Principalmente para a navegação
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private SearchInfo _searchInfo;
        private AuthenticationService _authenticationService;
        private ValidationService _validationService;
        public HomeController(EasyFitHubContext context, ILogger<HomeController> logger, AuthenticationService authenticationService, ValidationService validationService)
        {
            _logger = logger;
            _searchInfo = new SearchInfo(context, logger);
            _authenticationService = authenticationService;
            _validationService = validationService;
        }
        /// <summary>
        /// Displays the index page.
        /// </summary>
        /// <returns>Returns the appropriate view based on the user's authentication status.</returns>
        public async Task<IActionResult> Index()
        {
            var account = await _authenticationService.GetAccount("token", HttpContext.Session.GetString("AccessToken"));

            ViewData["AccountId"] = (account == null) ? null : account.AccountId;
            ViewData["AccountType"] = (account == null) ? null : account.AccountType;
            return View();
        }

        /// <summary>
        /// Displays a list of gyms based on search criteria.
        /// </summary>
        /// <param name="searchType">The type of search (location, name, etc.).</param>
        /// <param name="search">The search query.</param>
        /// <param name="subscribed">A flag indicating whether to show subscribed gyms only.</param>
        /// <returns>Returns the appropriate view based on search results.</returns>
        public async Task<IActionResult> GymList(string? searchType, string? search, string subscribed)
        {
            List<Gym>? list = null;
            var account = await _authenticationService.GetAccount("token", HttpContext.Session.GetString("AccessToken"));

            bool isSubscribed = subscribed == "on";

            if (account == null) return RedirectToAction("Index");

            if (isSubscribed)
            {
                list = await _searchInfo.GetGymsBySubscription(account);
            }
            else
            {
                switch (searchType)
                {
                    case "location":
                        if (_validationService.ValidateString(search, 60))
                            list = await _searchInfo.GetGymsByLocation(search);
                        break;
                    case "name":
                        if (_validationService.ValidateString(search, 60))
                            list = await _searchInfo.GetGymsByName(search);
                        break;
                    default:
                        list = await _searchInfo.GetGyms();
                        break;
                }
            }
            
            return View((list == null)? new List<Gym>(): list);
        }

        /// <summary>
        /// Redirects to the index page of the user's gym.
        /// </summary>
        /// <returns>Returns the appropriate view based on authorization status.</returns>
        public async Task<IActionResult> MyGym()
        {
            var account = await _authenticationService.GetAccount("token", HttpContext.Session.GetString("AccessToken"));
            if (account == null || account.AccountType != Models.Account.AccountType.MANAGER) return RedirectToAction("Index");

            var manager = account as Models.Account.Manager;
            return RedirectToAction("Index", "Gyms", new {gymId = manager.GymId });
        }




        /// <summary>
        /// Redirects the user to the gym list or authentication page based on authentication status.
        /// </summary>
        /// <returns>Returns the appropriate view based on authentication status.</returns>

        public async Task<IActionResult> GetStarted() {
            var account = await _authenticationService.GetAccount(
                "token", 
                HttpContext.Session.GetString("AccessToken")
            );

            if(account != null)
            {
                return RedirectToAction("GymList");
            }
            else
            {
                return RedirectToAction("Index", "Authentication");
            }
        }
        /// <summary>
        /// Displays the error page.
        /// </summary>
        /// <returns>Returns the error view.</returns>

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
