using Microsoft.AspNetCore.Mvc;
using EasyFitHub.Data;
using EasyFitHub.Models.Gym;
using EasyFitHub.Models.Account;
using EasyFitHub.Services;
using EasyFitHub.Models.Profile;
using System;
using EasyFitHub.Models.Miscalenous;


namespace EasyFitHub.Controllers
{
    /// <summary>
    /// Author: Rui Barroso
    /// Controlador usado para gerir Ginásios
    /// </summary>
    public class GymsController : Controller
    {
        private readonly GymsInfo _gymsInfo;
        private readonly AuthenticationService _authenticationService;
        private readonly AuthorizationService _authorizationService;
        private readonly ILogger<GymsController> _logger;
        private readonly FirebaseService _firebaseService;

        public GymsController(EasyFitHubContext context, AuthenticationService authenticationService, AuthorizationService authorizationService, ILogger<GymsController> logger, FirebaseService firebaseService)
        {
            _gymsInfo = new GymsInfo(context, logger);
            _authenticationService = authenticationService;
            _logger = logger;
            _authorizationService = authorizationService;
            _firebaseService = firebaseService;
        }

        /// <summary>
        /// Displays the gym details based on the provided gym ID.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <returns>Returns the appropriate view based on authorization status.</returns>
        // GET: Gyms?gymName=GymName
        public async Task<IActionResult> Index(int gymId)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);
            _logger.LogInformation($"\n\n\n\n AQUI: {result.Gym.Images.Count}\n\n\n\n");
            if (result.IsAuthorizedToEdit)
            {
                ViewData["accountType"] = result.Account!.AccountType;
                return View("Edit", result.Gym);
            }

            if (result.IsAuthorizedToRead) ViewData["isMember"] = true;


            return View(result.Gym);
        }

        /// <summary>
        /// Displays the employees of the gym based on the provided gym ID.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <returns>Returns the appropriate view based on authorization status.</returns>
        public async Task<IActionResult> Employees(int gymId)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.IsAuthorizedToEdit)
                return View("EmployeesEdit", result.Gym);

            if (result.IsAuthorizedToRead)
                return View(result.Gym);



            return RedirectToAction("Index", new { gymId = gymId });
        }// <summary>
        /// Displays the clients of the gym based on the provided gym ID.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <returns>Returns the appropriate view based on authorization status.</returns>
        public async Task<IActionResult> Clients(int gymId)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.IsAuthorizedToEdit)
                return View("ClientsEdit", result.Gym);

            if (result.IsAuthorizedToRead)
                return View(result.Gym);



            return RedirectToAction("Index", new { gymId = gymId });
        }
        /// <summary>
        /// Displays the requests of the gym based on the provided gym ID.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <returns>Returns the appropriate view based on authorization status.</returns>
        public async Task<IActionResult> Requests(int gymId)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.IsAuthorizedToEdit)
                return View("RequestsEdit", result.Gym);

            if (result.IsAuthorizedToRead)
                return View(result.Gym);


            return RedirectToAction("NotAuthorized");
        }

        /// <summary>
        /// Redirects to the Relationships action of a specific employee.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="gymEmployeeId">The ID of the gym employee.</param>
        /// <returns>Returns the appropriate view based on authorization status.</returns>
        [HttpPost]
        public IActionResult RelationshipsGymEmployee(int gymId, int? gymEmployeeId)
        {
            return RedirectToAction("Relationships", new { gymId = gymId, gymEmployeeId = gymEmployeeId });
        }
        /// <summary>
        /// Displays the relationships between gym employees and clients based on the provided gym ID and employee ID.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="gymEmployeeId">The ID of the gym employee.</param>
        /// <returns>Returns the appropriate view based on authorization status.</returns>
        public async Task<IActionResult> Relationships(int gymId, int? gymEmployeeId)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null) return View("NotAuthorized");

            IEnumerable<GymClient>? clients = null;
            int? employAccountId = null;
            if (gymEmployeeId != null)
            {
                var employee = await _gymsInfo.GetGymEmployee((int)gymEmployeeId);
                clients = employee.GymClients.Select(rel => rel.GymClient);
                employAccountId = employee.Client.UserId;
            }


            if (result.IsAuthorizedToEdit)
            {
                ViewData["EmployeeClients"] = clients;
                ViewData["EmployeeAccountId"] = employAccountId;
                return View("RelationshipsEdit", result.Gym);
            }

            if (result.IsAuthorizedToRead)
            {
                ViewData["EmployeeClients"] = clients;
                ViewData["EmployeeAccountId"] = employAccountId;
                return View(result.Gym);
            }



            return View("NotAuthorized");
        }


        /// <summary>
        /// Displays the gym edit page based on the provided gym ID.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <returns>Returns the appropriate view based on authorization status.</returns>
        public async Task<IActionResult> EditGym(int gymId)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.IsAuthorizedToEdit)
                return View(result.Gym);

            return View("NotAuthorized");
        }

        /// <summary>
        /// Handles the gym edit post request.
        /// </summary>
        /// <param name="newGym">The updated gym information.</param>
        /// <returns>Returns the appropriate view based on authorization status.</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(Gym? newGym)
        {
            if (newGym == null) return View("NotAuthorized");

            var result = await _authorizationService.isAuthorized(newGym.Id, HttpContext.Session);


            if (!result.IsAuthorizedToEdit)
                return View("NotAuthorized");

            await _gymsInfo.UpdateGym(newGym);

            return RedirectToAction("Index", new { gymId = newGym.Id });

        }


        /// <summary>
        /// Enlists a gym for the logged-in user.
        /// </summary>
        /// <param name="gymId">The ID of the gym to enlist.</param>
        /// <returns>Returns the appropriate view based on authorization status.</returns>
        public async Task<IActionResult> EnlistGym(int gymId)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || result.Account == null || result.Account.AccountType != AccountType.USER)
                return RedirectToAction("NotAuthorized");


            var client = await _gymsInfo.GetClient((User)result.Account);
            if (client == null) return RedirectToAction("NotAuthorized");

            await _gymsInfo.AddRequest(result.Gym, client);

            return RedirectToAction("Index", new { gymId = gymId });
        }
        /// <summary>
        /// Confirms a request for membership to the gym.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="accountId">The ID of the account requesting membership.</param>
        /// <param name="role">The role of the account in the gym.</param>
        /// <returns>Returns the appropriate view based on authorization status.</returns>
        public async Task<IActionResult> ConfirmRequest(int gymId, int accountId, Role role)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || result.Account == null || !result.IsAuthorizedToEdit)
                return RedirectToAction("NotAuthorized");

            var accountToConfirm = await _authenticationService.GetAccount("id", accountId.ToString());
            if (accountToConfirm == null)
            {
                TempData["ResultMessage"] = "Account was not Confirmed! Something went Wrong!";
                return RedirectToAction("Requests", new { gymId = gymId });
            }


            var client = await _gymsInfo.GetClient((User)accountToConfirm);
            if (client == null)
            {
                TempData["ResultMessage"] = "Account was not Confirmed! Something went Wrong!";
                return RedirectToAction("Requests", new { gymId = gymId });
            }



            if (role == Role.CLIENT)
            {
                await _gymsInfo.AddClient(client, result.Gym);
            }
            else
            {
                await _gymsInfo.AddEmployee(client, result.Gym, role);
            }

            await _gymsInfo.RemoveRequest(result.Gym, client);

            return RedirectToAction("Requests", new { gymId = gymId });
        }

        /// <summary>
        /// Denies a request for membership to the gym.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="accountId">The ID of the account requesting membership.</param>
        /// <returns>Returns the appropriate view based on authorization status.</returns>
        public async Task<IActionResult> DenyRequest(int gymId, int accountId)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || result.Account == null || !result.IsAuthorizedToEdit)
                return RedirectToAction("NotAuthorized");

            var accountToConfirm = await _authenticationService.GetAccount("id", accountId.ToString());
            if (accountToConfirm == null)
            {
                TempData["ResultMessage"] = "Account was not Confirmed! Something went Wrong!";
                return RedirectToAction("Requests", new { gymId = gymId });
            }


            var client = await _gymsInfo.GetClient((User)accountToConfirm);
            if (client == null)
            {
                TempData["ResultMessage"] = "Account was not Confirmed! Something went Wrong!";
                return RedirectToAction("Requests", new { gymId = gymId });
            }

            await _gymsInfo.RemoveRequest(result.Gym, client);

            return RedirectToAction("Requests", new { gymId = gymId });
        }


        /// <summary>
        /// Removes a client from the gym.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="accountId">The ID of the account to remove.</param>
        /// <returns>Returns the appropriate view based on authorization status.</returns>
        public async Task<IActionResult> RemoveClient(int gymId, int accountId)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || result.Account == null || (!result.IsAuthorizedToEdit && result.Account.AccountId != accountId))
                return RedirectToAction("NotAuthorized");

            var accountToRemove = await _authenticationService.GetAccount("id", accountId.ToString());
            if (accountToRemove == null)
            {
                TempData["ResultMessage"] = "Account was not Confirmed! Something went Wrong!";
                return RedirectToAction("Clients", new { gymId = gymId });
            }


            var client = await _gymsInfo.GetClient((User)accountToRemove);
            if (client == null)
            {
                TempData["ResultMessage"] = "Account was not Confirmed! Something went Wrong!";
                return RedirectToAction("Clients", new { gymId = gymId });
            }

            await _gymsInfo.RemoveClient(client, result.Gym);

            return RedirectToAction("Clients", new { gymId = gymId });
        }


        /// <summary>
        /// Removes an employee from the gym.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="accountId">The ID of the account to remove.</param>
        /// <returns>Returns the appropriate view based on authorization status.</returns>
        public async Task<IActionResult> RemoveEmployee(int gymId, int accountId)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || result.Account == null || (!result.IsAuthorizedToEdit && result.Account.AccountId != accountId))
                return RedirectToAction("NotAuthorized");

            var accountToRemove = await _authenticationService.GetAccount("id", accountId.ToString());
            if (accountToRemove == null)
            {
                TempData["ResultMessage"] = "Account was not Confirmed! Something went Wrong!";
                return RedirectToAction("Employees", new { gymId = gymId });
            }


            var employees = await _gymsInfo.GetClient((User)accountToRemove);
            if (employees == null)
            {
                TempData["ResultMessage"] = "Account was not Confirmed! Something went Wrong!";
                return RedirectToAction("Employees", new { gymId = gymId });
            }

            await _gymsInfo.RemoveEmployee(employees, result.Gym);

            return RedirectToAction("Employees", new { gymId = gymId });
        }
        /// <summary>
        /// Changes the role of an account in the gym.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="role">The new role for the user.</param>
        /// <returns>Returns the appropriate view based on authorization status.</returns>

        [HttpPost]
        public async Task<IActionResult> ChangeRole(int gymId, int userId, Role role)
        {
            return null;
        }

        /// <summary>
        /// Adds a relationship between a gym employee and a client.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="employeeAccountId">The ID of the employee account.</param>
        /// <param name="clientAccountId">The ID of the client account.</param>
        /// <returns>Returns the appropriate view based on authorization status.</returns>
        [HttpPost]
        public async Task<IActionResult> AddRelationship(int gymId, int employeeAccountId, int clientAccountId)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || result.Account == null || !result.IsAuthorizedToEdit)
                return RedirectToAction("NotAuthorized");

            var employeeAccount = await _authenticationService.GetAccount("id", employeeAccountId.ToString());
            var clientAccount = await _authenticationService.GetAccount("id", clientAccountId.ToString());

            if (employeeAccount == null || clientAccount == null)
            {
                TempData["ResultMessage"] = "Account was not Confirmed! Something went Wrong!";
                return RedirectToAction("Relationships", new { gymId = gymId });
            }

            var employee = await _gymsInfo.GetClient((User)employeeAccount);
            var client = await _gymsInfo.GetClient((User)clientAccount);

            if (employee == null || client == null)
            {
                TempData["ResultMessage"] = "Account was not Confirmed! Something went Wrong!";
                return RedirectToAction("Relationships", new { gymId = gymId });
            }

            await _gymsInfo.AddEmployeeClientRelation(employee, client, result.Gym);

            return RedirectToAction("Relationships", new { gymId = gymId });
        }

        /// <summary>
        /// Removes a relationship between a gym employee and a client.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="employeeAccountId">The ID of the employee account.</param>
        /// <param name="clientAccountId">The ID of the client account.</param>
        /// <returns>Returns the appropriate view based on authorization status.</returns>
        [HttpPost]
        public async Task<IActionResult> RemoveRelationship(int gymId, int employeeAccountId, int clientAccountId)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || result.Account == null || !result.IsAuthorizedToEdit)
                return RedirectToAction("NotAuthorized");

            var employeeAccount = await _authenticationService.GetAccount("id", employeeAccountId.ToString());
            var clientAccount = await _authenticationService.GetAccount("id", clientAccountId.ToString());

            if (employeeAccount == null || clientAccount == null)
            {
                TempData["ResultMessage"] = "Account was not Confirmed! Something went Wrong!";
                return RedirectToAction("Relationships", new { gymId = gymId });
            }

            var employee = await _gymsInfo.GetClient((User)employeeAccount);
            var client = await _gymsInfo.GetClient((User)clientAccount);

            if (employee == null || client == null)
            {
                TempData["ResultMessage"] = "Account was not Confirmed! Something went Wrong!";
                return RedirectToAction("Relationships", new { gymId = gymId });
            }



            var res = await _gymsInfo.RemoveEmployeeClientRelation(employee, client, result.Gym);

            return RedirectToAction("Relationships", new { gymId = gymId });
        }


        /// <summary>
        /// Displays the unconfirmed gyms for an admin user.
        /// </summary>
        /// <returns>Returns the appropriate view based on authorization status.</returns>
        public async Task<IActionResult> ConfirmGyms()
        {
            var account = await _authenticationService.GetAccount("token", HttpContext.Session.GetString("AccessToken"));
            if (account == null || account.AccountType != AccountType.ADMIN) return View("NotAuthorized");

            var gyms = await _gymsInfo.GetGyms();

            var filteredGyms = gyms.Where(g => g.IsConfirmed == false);

            return View(filteredGyms.ToList());
        }
        /// <summary>
        /// Displays the gyms available for deletion by an admin user.
        /// </summary>
        /// <returns>Returns the appropriate view based on authorization status.</returns>
        public async Task<IActionResult> DeleteGyms()
        {
            var account = await _authenticationService.GetAccount("token", HttpContext.Session.GetString("AccessToken"));
            if (account == null || account.AccountType != AccountType.ADMIN) return View("NotAuthorized");

            var gyms = await _gymsInfo.GetGyms();

            return View(gyms.ToList());
        }

        /// <summary>
        /// Confirms or deletes a gym based on the provided parameters.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="isConfirm">A flag indicating whether to confirm or delete the gym.</param>
        /// <returns>Returns the appropriate view based on authorization status.</returns>
        [HttpPost]
        public async Task<IActionResult> ConfirmGym(int gymId, bool isConfirm)
        {
            var account = await _authenticationService.GetAccount("token", HttpContext.Session.GetString("AccessToken"));
            if (account == null || account.AccountType != AccountType.ADMIN) return View("NotAuthorized");

            bool res = false;
            if (isConfirm)
                res = await _gymsInfo.ConfirmGym(gymId);
            else
                res = await _gymsInfo.DeleteGym(gymId);

            TempData["ResultMessage"] = (res) ? null : "Something Went Wrong!";
            return RedirectToAction("ConfirmGyms");
        }

        /// <summary>
        /// Deletes a gym based on the provided gym ID.
        /// </summary>
        /// <param name="gymId">The ID of the gym to delete.</param>
        /// <returns>Returns the appropriate view based on authorization status.</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteGym(int gymId, bool isRedirected = false)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);
            if (!result.IsAuthorizedToEdit || result.Account!.AccountType == AccountType.USER) return View("NotAuthorized");

            if (!isRedirected) return RedirectToAction("UnsubscribeAll", new { gymId = gymId });

            var res = await _gymsInfo.DeleteGym(gymId);

            TempData["ResultMessage"] = (res) ? null : "Something Went Wrong!";
            if (result.Account!.AccountType == AccountType.MANAGER)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("DeleteGyms");
        }


        /// <summary>
        /// Handles the image file upload for a gym.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="file">The file to upload.</param>
        /// <returns>Returns the appropriate view based on authorization status.</returns>

        [HttpPost]
        public async Task<IActionResult> UploadFile(int gymId, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);
                if (!result.IsAuthorizedToEdit || result.Account!.AccountType == AccountType.USER) return View("NotAuthorized");

                HubImage? newHubImg = await _gymsInfo.CreateImage(gymId);

                using (var stream = file.OpenReadStream())
                {
                    newHubImg = await _firebaseService.CreateHubImage(newHubImg, stream, _logger);
                }
                await _gymsInfo.UpdatePathImage(gymId, newHubImg.Name, newHubImg.Path);

                return RedirectToAction("Index", new { gymId = gymId });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Handles the deletion of a file associated with a gym.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="imageName">The name of the image to delete.</param>
        /// <returns>Returns the appropriate view based on authorization status.</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteFile(int gymId, string imageName)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);
            if (!result.IsAuthorizedToEdit || result.Account!.AccountType == AccountType.USER) return View("NotAuthorized");

            var image = await _gymsInfo.RemoveImage(gymId, imageName);

            var res = await _firebaseService.DeleteHubImage(image!);

            return RedirectToAction("Index", new { gymId = gymId });
        }
    }
}
