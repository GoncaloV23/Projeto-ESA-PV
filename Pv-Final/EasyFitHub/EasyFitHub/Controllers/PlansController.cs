using EasyFitHub.Data;
using EasyFitHub.Models.Account;
using EasyFitHub.Models.Gym;
using EasyFitHub.Models.Inventory;
using EasyFitHub.Models.Plan;
using EasyFitHub.Models.Profile;
using EasyFitHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Configuration;
using System;

namespace EasyFitHub.Controllers
{
    public class PlansController : Controller
    {
        /// <summary>
        /// Author: Gonçalo V.
        /// Controlador destinado á gestão de Planos (De exercicios ou de Dieta)
        /// PlanInfo é usado para efetuar operações no contexto
        /// ProfilesInfo é usado para verificar autorizações do utilizador autenticado
        /// authenticationService é usado para verificar a autenticação
        /// firebaseService é usado para a gestão de Imagens nos diferentes itens do modelo
        /// </summary>

        private readonly AuthenticationService _authenticationService;
        private readonly ILogger<PlansController> _logger;
        private readonly PlansInfo _planInfo;
        private readonly ProfilesInfo _profilesInfo;
        private readonly FirebaseService _firebaseService;

        public PlansController(AuthenticationService authenticationService, ILogger<PlansController> logger, EasyFitHubContext context, FirebaseService firebaseService)
        {
            _authenticationService = authenticationService;
            _logger = logger;
            _planInfo = new PlansInfo(context, logger);
            _profilesInfo = new ProfilesInfo(context, logger);
            _firebaseService = firebaseService;
        }
        /// <summary>
        /// Retrieves the index view for displaying user plans.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The index view.</returns>
        public async Task<IActionResult> Index(int userId)
        {
            var sessionAccount = await GetSessionAccount();
            var account = await GetAccount(userId);

            if (sessionAccount == null || account == null) return RedirectToAction("Index", "Home");

            var client = _profilesInfo.GetUser(account.AccountId);
            var sessionClient = _profilesInfo.GetUser(sessionAccount.AccountId);

            var result = sessionAccount.AccountId != account.AccountId && !IsClientOf(sessionClient, client);

            if (result) return View("NotAuthorized");

            return View("Index", client);
        }
        /// <summary>
        /// Retrieves the view for displaying an exercise.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="exerciseId">The ID of the exercise.</param>
        /// <returns>The exercise view.</returns>
        public async Task<IActionResult> Exercise(int userId, int exerciseId)
        {
            var sessionAccount = await GetSessionAccount();
            var account = await GetAccount(userId);

            if (sessionAccount == null || account == null) return RedirectToAction("Index", "Home");

            var client = _profilesInfo.GetUser(account.AccountId);
            var sessionClient = _profilesInfo.GetUser(sessionAccount.AccountId);

            var result = sessionAccount.AccountId != account.AccountId && !IsClientOf(sessionClient, client);

            if (result) return View("NotAuthorized");

            var exercise = await _planInfo.GetExercise(exerciseId);

            if (exercise == null) return RedirectToAction("Index", "Home");

            return View(exercise);
        }
        /// <summary>
        /// Retrieves the view for creating a new exercise.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="planId">The ID of the plan.</param>
        /// <returns>The create exercise view.</returns>
        public async Task<IActionResult> CreateExercise(int userId, int planId)
        {
            var sessionAccount = await GetSessionAccount();
            var account = await GetAccount(userId);

            if (sessionAccount == null || account == null) return RedirectToAction("Index", "Home");

            var client = _profilesInfo.GetUser(account.AccountId);
            var sessionClient = _profilesInfo.GetUser(sessionAccount.AccountId);

            if (client == null || sessionClient == null) return RedirectToAction("Index", "Home");

            var result = sessionAccount.AccountId != account.AccountId && !IsClientOf(sessionClient, client);

            if (result) return View("NotAuthorized");

            var plan = await _planInfo.GetPlan(planId);
            if (plan == null) return RedirectToAction("Plans", new { userId = userId, planType = PlanType.NUTRITION });

            PlanExercise exercise = new PlanExercise
            {
                PlanId = planId,
                Plan = plan
            };

            var gymExercises = await _planInfo.GetExercises(client.ClientId, sessionClient.ClientId);

            ViewData["Exercises"] = gymExercises;

            return View(exercise);

            
        }
        /// <summary>
        /// Retrieves the view for updating an existing exercise.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="exerciseId">The ID of the exercise.</param>
        /// <returns>The update exercise view.</returns>
        public async Task<IActionResult> UpdateExercise(int userId, int exerciseId)
        {
            var sessionAccount = await GetSessionAccount();
            var account = await GetAccount(userId);

            if (sessionAccount == null || account == null) return RedirectToAction("Index", "Home");

            var client = _profilesInfo.GetUser(account.AccountId);
            var sessionClient = _profilesInfo.GetUser(sessionAccount.AccountId);

            if (client == null || sessionClient == null) return RedirectToAction("Index", "Home");

            var result = sessionAccount.AccountId != account.AccountId && !IsClientOf(sessionClient, client);

            if (result) return View("NotAuthorized");

            var exercise = await _planInfo.GetExercise(exerciseId);
            var gymExercises = await _planInfo.GetExercises(client.ClientId, sessionClient.ClientId);

            ViewData["Exercises"] = gymExercises;

            return View(exercise);
        }
        /// <summary>
        /// Retrieves the view for displaying a meal.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="mealId">The ID of the meal.</param>
        /// <returns>The meal view.</returns>
        public async Task<IActionResult> Meal(int userId, int mealId)
        {
            var sessionAccount = await GetSessionAccount();
            var account = await GetAccount(userId);

            if (sessionAccount == null || account == null) return RedirectToAction("Index", "Home");

            var client = _profilesInfo.GetUser(account.AccountId);
            var sessionClient = _profilesInfo.GetUser(sessionAccount.AccountId);

            var result = sessionAccount.AccountId != account.AccountId && !IsClientOf(sessionClient, client);

            if (result) return View("NotAuthorized");

            var meal = await _planInfo.GetMeal(mealId);

            if (meal == null) return RedirectToAction("Index", "Home");

            return View(meal);
        }
        /// <summary>
        /// Retrieves the view for creating a new meal.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="planId">The ID of the plan.</param>
        /// <returns>The create meal view.</returns>
        public async Task<IActionResult> CreateMeal(int userId, int planId)
        {
            var sessionAccount = await GetSessionAccount();
            var account = await GetAccount(userId);

            if (sessionAccount == null || account == null) return RedirectToAction("Index", "Home");

            var client = _profilesInfo.GetUser(account.AccountId);
            var sessionClient = _profilesInfo.GetUser(sessionAccount.AccountId);

            var result = sessionAccount.AccountId != account.AccountId && !IsClientOf(sessionClient, client);

            if (result) return View("NotAuthorized");

            var plan = await _planInfo.GetPlan(planId);
            if(plan == null)return RedirectToAction("Plans", new {userId = userId , planType = PlanType.NUTRITION});

            PlanMeal meal = new PlanMeal
            {
                PlanId = planId,
                Plan = plan
            };

            return View(meal);
        }
        /// <summary>
        /// Retrieves the view for updating an existing meal.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="mealId">The ID of the meal.</param>
        /// <returns>The update meal view.</returns>
        public async Task<IActionResult> UpdateMeal(int userId, int mealId)
        {
            var sessionAccount = await GetSessionAccount();
            var account = await GetAccount(userId);

            if (sessionAccount == null || account == null) return RedirectToAction("Index", "Home");

            var client = _profilesInfo.GetUser(account.AccountId);
            var sessionClient = _profilesInfo.GetUser(sessionAccount.AccountId);

            if (client == null || sessionClient == null) return RedirectToAction("Index", "Home");

            var result = sessionAccount.AccountId != account.AccountId && !IsClientOf(sessionClient, client);

            if (result) return View("NotAuthorized");

            var meal = await _planInfo.GetMeal(mealId);

            if (meal == null) return RedirectToAction("Index", "Home");

            return View(meal);
        }
        /// <summary>
        /// Retrieves the view for displaying a plan.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="planId">The ID of the plan.</param>
        /// <returns>The plan view.</returns>
        public async Task<IActionResult> Plan(int userId, int planId)
        {
            var sessionAccount = await GetSessionAccount();
            var account = await GetAccount(userId);

            if (sessionAccount == null || account == null) return RedirectToAction("Index", "Home");

            var client = _profilesInfo.GetUser(account.AccountId);
            var sessionClient = _profilesInfo.GetUser(sessionAccount.AccountId);

            if (client == null || sessionClient == null) return RedirectToAction("Index", "Home");

            var result = sessionAccount.AccountId != account.AccountId && !IsClientOf(sessionClient, client);

            if (result) return View("NotAuthorized");

            var plan = await _planInfo.GetPlan(planId);

            if (plan == null) return RedirectToAction("Index", "Home");

            return View(plan);
        }
        /// <summary>
        /// Retrieves the view for creating a new plan.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="planType">The type of the plan (exercise or nutrition).</param>
        /// <returns>The create plan view.</returns>
        public async Task<IActionResult> CreatePlan(int userId, PlanType planType)
        {
            var sessionAccount = await GetSessionAccount();
            var account = await GetAccount(userId);

            if (sessionAccount == null || account == null) return RedirectToAction("Index", "Home");

            var client = _profilesInfo.GetUser(account.AccountId);
            var sessionClient = _profilesInfo.GetUser(sessionAccount.AccountId);

            if (client == null || sessionClient == null) return RedirectToAction("Index", "Home");

            var result = sessionAccount.AccountId != account.AccountId && !IsClientOf(sessionClient, client);

            if (result) return View("NotAuthorized");

            Plan? plan = new Plan { 
                    PlanType = planType,
                    Client = client,
                    ClientId = client.ClientId
                };

            return View(plan);
        }
        /// <summary>
        /// Retrieves the view for updating an existing plan.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="planId">The ID of the plan.</param>
        /// <returns>The update plan view.</returns>
        public async Task<IActionResult> UpdatePlan(int userId, int planId)
        {
            var sessionAccount = await GetSessionAccount();
            var account = await GetAccount(userId);

            if (sessionAccount == null || account == null) return RedirectToAction("Index", "Home");

            var client = _profilesInfo.GetUser(account.AccountId);
            var sessionClient = _profilesInfo.GetUser(sessionAccount.AccountId);

            if (client == null || sessionClient == null) return RedirectToAction("Index", "Home");

            var result = sessionAccount.AccountId != account.AccountId && !IsClientOf(sessionClient, client);

            if (result) return View("NotAuthorized");

            var plan = await _planInfo.GetPlan(planId);

            if (plan == null) return RedirectToAction("Index", "Home");

            return View(plan);
        }
        /// <summary>
        /// Retrieves the view for displaying plans of a specific type.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="planType">The type of the plan (exercise or nutrition).</param>
        /// <returns>The plans view.</returns>
        public async Task<IActionResult> Plans(int userId, PlanType planType)
        {
            var sessionAccount = await GetSessionAccount();
            var account = await GetAccount(userId);

            if (sessionAccount == null || account == null) return RedirectToAction("Index", "Home");

            var client = _profilesInfo.GetUser(account.AccountId);
            var sessionClient = _profilesInfo.GetUser(sessionAccount.AccountId);

            if (client == null || sessionClient == null) return RedirectToAction("Index", "Home");

            var result = sessionAccount.AccountId != account.AccountId && !IsClientOf(sessionClient, client);

            if (result) return View("NotAuthorized");

            List<Plan>? plans = null;

            if (planType == PlanType.NUTRITION)
            {
                plans = await _planInfo.GetMealPlans(client.ClientId);
            }
            else if (planType == PlanType.EXERCISE)
            {
                plans = await _planInfo.GetExercisePlans(client.ClientId);
            }

            ViewData["PlanType"] = planType;
            ViewData["UserId"] = userId;

            return View(plans);
        }
        /// <summary>
        /// Creates a new exercise associated with a plan.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="planId">The ID of the plan.</param>
        /// <param name="exercise">The exercise object.</param>
        /// <returns>A redirect to the plan view.</returns>

        [HttpPost]
        public async Task<IActionResult> CreateExercise(int userId, int planId, PlanExercise exercise)
        {
            var sessionAccount = await GetSessionAccount();
            var account = await GetAccount(userId);

            if (sessionAccount == null || account == null) return RedirectToAction("Index", "Home");

            var client = _profilesInfo.GetUser(account.AccountId);
            var sessionClient = _profilesInfo.GetUser(sessionAccount.AccountId);

            if (client == null || sessionClient == null) return RedirectToAction("Index", "Home");

            var result = sessionAccount.AccountId != account.AccountId && !IsClientOf(sessionClient, client);

            if (result) return View("NotAuthorized");

            await _planInfo.CreateExercise(planId, exercise);

            return RedirectToAction("Plan", new { userId = userId, planId = planId });
        }
        /// <summary>
        /// Updates an existing exercise.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="exercise">The updated exercise object.</param>
        /// <returns>A redirect to the plan view.</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateExercise(int userId, PlanExercise exercise)
        {
            var sessionAccount = await GetSessionAccount();
            var account = await GetAccount(userId);

            if (sessionAccount == null || account == null) return RedirectToAction("Index", "Home");

            var client = _profilesInfo.GetUser(account.AccountId);
            var sessionClient = _profilesInfo.GetUser(sessionAccount.AccountId);

            if (client == null || sessionClient == null) return RedirectToAction("Index", "Home");

            var result = sessionAccount.AccountId != account.AccountId && !IsClientOf(sessionClient, client);

            if (result) return View("NotAuthorized");

            await _planInfo.UpdateExercise(userId, exercise);

            return RedirectToAction("Plan", new { userId = userId, planId = exercise.PlanId });
        }
        /// <summary>
        /// Deletes an existing exercise.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="exerciseId">The ID of the exercise to delete.</param>
        /// <returns>A redirect to the plan view.</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteExercise(int userId, int exerciseId)
        {
            var sessionAccount = await GetSessionAccount();
            var account = await GetAccount(userId);

            if (sessionAccount == null || account == null) return RedirectToAction("Index", "Home");

            var client = _profilesInfo.GetUser(account.AccountId);
            var sessionClient = _profilesInfo.GetUser(sessionAccount.AccountId);

            if (client == null || sessionClient == null) return RedirectToAction("Index", "Home");

            var result = sessionAccount.AccountId != account.AccountId && !IsClientOf(sessionClient, client);

            if (result) return View("NotAuthorized");

            var exercise = await _planInfo.DeleteExercise(exerciseId);

            return RedirectToAction("Plan", new { userId = userId, planId = exercise.PlanId });
        }

        /// <summary>
        /// Creates a new meal associated with a plan.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="planId">The ID of the plan.</param>
        /// <param name="file">The image file associated with the meal.</param>
        /// <param name="meal">The meal object.</param>
        /// <returns>A redirect to the plan view.</returns>

        [HttpPost]
        public async Task<IActionResult> CreateMeal(int userId, int planId, IFormFile file, PlanMeal meal)
        {
            var sessionAccount = await GetSessionAccount();
            var account = await GetAccount(userId);

            if (sessionAccount == null || account == null) return RedirectToAction("Index", "Home");

            var client = _profilesInfo.GetUser(account.AccountId);
            var sessionClient = _profilesInfo.GetUser(sessionAccount.AccountId);

            if (client == null || sessionClient == null) return RedirectToAction("Index", "Home");

            var result = sessionAccount.AccountId != account.AccountId && !IsClientOf(sessionClient, client);

            if (result) return View("NotAuthorized");

            var createdMeal = await _planInfo.CreateMeal(planId, meal);

            if (createdMeal != null && file != null && file.Length > 0)
                return await UploadMealImage(createdMeal.PlanItemId, userId, file);

            return RedirectToAction("Plan", new { userId = userId, planId = planId });
        }
        /// <summary>
        /// Updates an existing meal.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="meal">The updated meal object.</param>
        /// <returns>A redirect to the plan view.</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateMeal(int userId, PlanMeal meal)
        {
            var sessionAccount = await GetSessionAccount();
            var account = await GetAccount(userId);

            if (sessionAccount == null || account == null) return RedirectToAction("Index", "Home");

            var client = _profilesInfo.GetUser(account.AccountId);
            var sessionClient = _profilesInfo.GetUser(sessionAccount.AccountId);

            if (client == null || sessionClient == null) return RedirectToAction("Index", "Home");

            var result = sessionAccount.AccountId != account.AccountId && !IsClientOf(sessionClient, client);

            if (result) return View("NotAuthorized");

            await _planInfo.UpdateMeal(userId, meal);

            return RedirectToAction("Plan", new { userId = userId, planId = meal.PlanId });
        }
        /// <summary>
        /// Deletes an existing meal.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="mealId">The ID of the meal to delete.</param>
        /// <returns>A redirect to the plan view.</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteMeal(int userId, int mealId)
        {
            var sessionAccount = await GetSessionAccount();
            var account = await GetAccount(userId);

            if (sessionAccount == null || account == null) return RedirectToAction("Index", "Home");

            var client = _profilesInfo.GetUser(account.AccountId);
            var sessionClient = _profilesInfo.GetUser(sessionAccount.AccountId);

            if (client == null || sessionClient == null) return RedirectToAction("Index", "Home");

            var result = sessionAccount.AccountId != account.AccountId && !IsClientOf(sessionClient, client);

            if (result) return View("NotAuthorized");

            var exercise = await _planInfo.DeleteMeal(mealId);

            return RedirectToAction("Plan", new { userId = userId, planId = exercise.PlanId });
        }
        /// <summary>
        /// Creates a new plan.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="file">The image file associated with the plan.</param>
        /// <param name="plan">The plan object.</param>
        /// <returns>A redirect to the plans view.</returns>

        [HttpPost]
        public async Task<IActionResult> CreatePlan(int userId, IFormFile file, Plan plan)
        {
            var sessionAccount = await GetSessionAccount();
            var account = await GetAccount(userId);

            if (sessionAccount == null || account == null) return RedirectToAction("Index", "Home");

            var client = _profilesInfo.GetUser(account.AccountId);
            var sessionClient = _profilesInfo.GetUser(sessionAccount.AccountId);

            if (client == null || sessionClient == null) return RedirectToAction("Index", "Home");

            var result = sessionAccount.AccountId != account.AccountId && !IsClientOf(sessionClient, client);

            if (result) return View("NotAuthorized");

            var createdPlan = await _planInfo.CreatePlan(userId, plan);

            if (createdPlan != null && file != null && file.Length > 0)
                return await UploadPlanImage(createdPlan.PlanId, userId, file);

            return RedirectToAction("Plans", new { userId = userId, planType = plan.PlanType});
        }
        /// <summary>
        /// Updates an existing plan.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="plan">The updated plan object.</param>
        /// <returns>A redirect to the plan view.</returns>

        [HttpPost]
        public async Task<IActionResult> UpdatePlan(int userId, Plan plan)
        {
            var sessionAccount = await GetSessionAccount();
            var account = await GetAccount(userId);

            if (sessionAccount == null || account == null) return RedirectToAction("Index", "Home");

            var client = _profilesInfo.GetUser(account.AccountId);
            var sessionClient = _profilesInfo.GetUser(sessionAccount.AccountId);

            if (client == null || sessionClient == null) return RedirectToAction("Index", "Home");

            var result = sessionAccount.AccountId != account.AccountId && !IsClientOf(sessionClient, client);

            if (result) return View("NotAuthorized");

            await _planInfo.UpdatePlan(userId, plan);

            return RedirectToAction("Plan", new { userId = userId, planId = plan.PlanId });
        }
        /// <summary>
        /// Deletes an existing plan.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="planId">The ID of the plan to delete.</param>
        /// <param name="planType">The type of the plan (exercise or nutrition).</param>
        /// <returns>A redirect to the plans view.</returns>

        [HttpPost]
        public async Task<IActionResult> DeletePlan(int userId, int planId, PlanType planType)
        {
            var sessionAccount = await GetSessionAccount();
            var account = await GetAccount(userId);

            if (sessionAccount == null || account == null) return RedirectToAction("Index", "Home");

            var client = _profilesInfo.GetUser(account.AccountId);
            var sessionClient = _profilesInfo.GetUser(sessionAccount.AccountId);

            if (client == null || sessionClient == null) return RedirectToAction("Index", "Home");

            var result = sessionAccount.AccountId != account.AccountId && !IsClientOf(sessionClient, client);

            if (result) return View("NotAuthorized");

            var res = await _planInfo.DeletePlan(planId);

            return RedirectToAction("Plans", new { userId = userId, planType = planType });
        }


        /// <summary>
        /// Retrieves the view for displaying employees of a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The employees view.</returns>
        public async Task<IActionResult> Employees(int userId)
        {
            var client = _profilesInfo.GetUser(userId);
            var employees = _profilesInfo.GetEmployees(client);

            return View(employees);
        }
        /// <summary>
        /// Retrieves the view for displaying employees of a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The clients view.</returns>
        public async Task<IActionResult> Clients(int userId)
        {
            var user = _profilesInfo.GetUser(userId);
            var client = _profilesInfo.GetEmployees(user);

            return View(client);
        }


        /// <summary>
        /// Checks if a client is associated with an Employee.
        /// </summary>
        /// <param name="employee">The client to check against.</param>
        /// <param name="user">The user/client to check.</param>
        /// <returns>True if the client is associated with another client, otherwise false.</returns>

        private bool IsClientOf(Client employee, Client user)
        {
            return _profilesInfo.GetEmployees(user).Any(e => e.ClientId == employee.ClientId);
        }
        /// <summary>
        /// Devolve a conta autenticada
        /// </summary>
        /// <returns>Account instance</returns>
        private async Task<Account?> GetSessionAccount()
        {
            return await _authenticationService.GetAccount("token", HttpContext.Session.GetString("AccessToken"));
        }
        /// <summary>
        /// retorna uma conta atravez do seu Id
        /// </summary>
        /// <param name="userId">Id da conta do User</param>
        /// <returns>Account instance</returns>
        private async Task<Account?> GetAccount(int userId)
        {
            return await _authenticationService.GetAccount("id", userId.ToString());
        }


        /// <summary>
        /// uploads a new image to a Meal
        /// </summary>
        /// <param name="mealId">Id of the Meal</param>
        /// <param name="userId">Id of the User with a Plan</param>
        /// <param name="file">file of the image</param>
        /// <returns>redirects to UpdateMeal Action</returns>
        [HttpPost]
        public async Task<IActionResult> UploadMealImage(int mealId, int userId, IFormFile file)
        {
            if (file == null || file.Length <= 0) return RedirectToAction("Index", "Home");

            var sessionAccount = await GetSessionAccount();
            var account = await GetAccount(userId);

            if (sessionAccount == null || account == null) return RedirectToAction("Index", "Home");

            var client = _profilesInfo.GetUser(account.AccountId);
            var sessionClient = _profilesInfo.GetUser(sessionAccount.AccountId);

            if (client == null || sessionClient == null) return RedirectToAction("Index", "Home");

            var result = sessionAccount.AccountId != account.AccountId && !IsClientOf(sessionClient, client);

            if (result) return View("NotAuthorized", "Home");

            var mealImg = await _planInfo.CreateMealImage(mealId);

            using (var stream = file.OpenReadStream())
            {
                mealImg = await _firebaseService.CreateHubImage(mealImg, stream, _logger);
            }

            if (mealImg != null) await _planInfo.UpdatePathImage(mealImg.Name, mealImg.Path);

            return RedirectToAction("UpdateMeal", new { userId = userId,  mealId = mealId });

        }
        /// <summary>
        /// uploads a new image to a Plan
        /// </summary>
        /// <param name="planId">Id of the Plan</param>
        /// <param name="userId">Id of the User</param>
        /// <param name="file">File of the image</param>
        /// <returns>redirects to UpdatePlan Action</returns>
        [HttpPost]
        public async Task<IActionResult> UploadPlanImage(int planId, int userId, IFormFile file)
        {
            if (file == null || file.Length <= 0) return RedirectToAction("Index", "Home");

            var sessionAccount = await GetSessionAccount();
            var account = await GetAccount(userId);

            if (sessionAccount == null || account == null) return RedirectToAction("Index", "Home");

            var client = _profilesInfo.GetUser(account.AccountId);
            var sessionClient = _profilesInfo.GetUser(sessionAccount.AccountId);

            if (client == null || sessionClient == null) return RedirectToAction("Index", "Home");

            var result = sessionAccount.AccountId != account.AccountId && !IsClientOf(sessionClient, client);

            if (result) return View("NotAuthorized", "Home");

            var planImg = await _planInfo.CreatePlanImage(planId);

            using (var stream = file.OpenReadStream())
            {
                planImg = await _firebaseService.CreateHubImage(planImg, stream, _logger);
            }

            if (planImg != null) await _planInfo.UpdatePathImage(planImg.Name, planImg.Path);

            return RedirectToAction("UpdatePlan", new { userId = userId, planId = planId });

        }
        /// <summary>
        /// Deletes an image File from FireBase
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="imageName"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteFile(int userId, string imageName)
        {
            var sessionAccount = await GetSessionAccount();
            var account = await GetAccount(userId);

            if (sessionAccount == null || account == null) return RedirectToAction("Index", "Home");

            var client = _profilesInfo.GetUser(account.AccountId);
            var sessionClient = _profilesInfo.GetUser(sessionAccount.AccountId);

            if (client == null || sessionClient == null) return RedirectToAction("Index", "Home");

            var result = sessionAccount.AccountId != account.AccountId && !IsClientOf(sessionClient, client);

            if (result) return View("NotAuthorized", "Home");

            var image = await _planInfo.RemoveImage(imageName);

            var res = await _firebaseService.DeleteHubImage(image!);

            return Content("<script>window.location.reload();</script>");
        }


        /*public async Task<ActionResult> Test(int i)
        {
            PlanMeal meal1 = new PlanMeal
            {
                Name = "MealTest1",
                Description = "Description Meal Test",
                HubImage = new Models.Miscalenous.HubImage
                {
                    Name = "Test",
                    Description = "Test desc",
                    Path = "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2Fimage_gym15e44f76e-51d6-43ec-baef-403018e2de76?alt=media&token=11443c70-d6f2-49b0-a7e9-a985eb1ddcbe"
                },
                Plan = new Plan
                {
                    Title = "Bulking Plan",
                    Description = "Test",
                    PlanType = PlanType.NUTRITION
                }
            };
            PlanMeal meal2 = new PlanMeal
            {
                Plan = new Plan
                {
                    Title = "Bulking Plan",
                    Description = "Test",
                    PlanType = PlanType.NUTRITION
                }
            };
            PlanMeal meal3 = new PlanMeal
            {
                Name = "MealTest1",
                Description = "Description Meal Test and this is Description Meal Test but also is Description Meal Test and also this is Description Meal TestDescription Meal Test",
                Plan = new Plan
                {
                    Title = "Bulking Plan",
                    Description = "Test",
                    PlanType = PlanType.NUTRITION
                }
            };
            if (i == 1) return View("UpdateMeal", meal1);
            else if (i == 2) return View("CreateMeal", meal2);
            else  return View("Meal", meal3);
        }*/
    }
}
