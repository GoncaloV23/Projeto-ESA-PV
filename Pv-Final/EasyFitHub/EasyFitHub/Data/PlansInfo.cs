using EasyFitHub.Models.Account;
using EasyFitHub.Models.Gym;
using EasyFitHub.Models.Inventory;
using EasyFitHub.Models.Miscalenous;
using EasyFitHub.Models.Plan;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.PortableExecutable;

namespace EasyFitHub.Data
{
    /// <summary>
    /// AUTHOR: Gonçalo V.
    /// Classe usada para efetuar operaçoes no contexto
    /// </summary>
    public class PlansInfo
    {
        private readonly EasyFitHubContext _context;
        private readonly ILogger? _logger;
        public PlansInfo(EasyFitHubContext context, ILogger? logger = null)
        {
            _context = context;
            _logger = logger;
        }
        /// <summary>
        /// Retrieves a list of gyms based on the provided client and employee IDs.
        /// </summary>
        /// <param name="clientId">The ID of the client.</param>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <returns>A task representing the asynchronous operation, returning a list of gyms.</returns>

        public async Task<List<Gym>> GetGyms(int clientId, int employeeId)
        {
            try
            {
                List<Gym> gyms = new List<Gym>();
                if (clientId == employeeId)
                    gyms = await _context.Gym
                    .Where(g => g.GymClients.Any(e => e.ClientId == clientId))
                    .ToListAsync();
                else 
                    gyms = await _context.Gym
                    .Where(g => g.GymEmployees.Any(e => e.ClientId == employeeId) && g.GymClients.Any(e => e.ClientId == clientId))
                    .ToListAsync();

                return gyms;
            }
            catch(Exception ex)
            {
                printMessage($"Error in GetGyms of PlansInfo: {ex.Message}");
                return new List<Gym>();
            }
        }
        /// <summary>
        /// Retrieves a list of exercises available at gyms accessible to the provided client and employee IDs.
        /// </summary>
        /// <param name="clientId">The ID of the client.</param>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <returns>A task representing the asynchronous operation, returning a list of exercises.</returns>

        public async Task<List<Exercise>> GetExercises(int clientId, int employeeId)
        {
            try
            {
                var gyms = (await GetGyms(clientId, employeeId)).Select(g => g.Id);

                var exercises = await _context.Exercises
                    .Include(e => e.Image)
                    .Where(e => gyms.Contains(e.Machine.GymId))
                    .ToListAsync();

                return exercises;
            }
            catch(Exception ex)
            {
                printMessage($"Error in GetExercises of PlansInfo: {ex.Message}");
                return new List<Exercise>();
            }
        }
        /// <summary>
        /// Retrieves a meal plan item based on the provided meal ID.
        /// </summary>
        /// <param name="mealId">The ID of the meal plan item.</param>
        /// <returns>A task representing the asynchronous operation, returning the meal plan item.</returns>

        public async Task<PlanMeal?> GetMeal(int mealId)
        {
            try
            {
                var meal = await _context.PlanItems.OfType<PlanMeal>()
                    .Include(m => m.HubImage)
                    .Include(m => m.Plan).ThenInclude(p => p.Client)
                    .SingleOrDefaultAsync(i => i.PlanItemId == mealId);

                return meal;
            }
            catch(Exception ex)
            {
                printMessage($"Error in GetMeal of PlansInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Creates a new meal plan item within the specified plan.
        /// </summary>
        /// <param name="planId">The ID of the plan to which the meal will be added.</param>
        /// <param name="meal">The meal plan item to be created.</param>
        /// <returns>A task representing the asynchronous operation, returning the created meal plan item.</returns>

        public async Task<PlanMeal?> CreateMeal(int planId, PlanMeal meal)
        {
            try
            {
                var plan = await GetPlan(planId);
                plan.Items.Add(meal);
                await _context.SaveChangesAsync();
                return meal;
            }
            catch(Exception ex)
            {
                printMessage($"Error in CreateMeal of PlansInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Updates an existing meal plan item within the specified plan.
        /// </summary>
        /// <param name="planId">The ID of the plan containing the meal to be updated.</param>
        /// <param name="meal">The updated meal plan item.</param>
        /// <returns>A task representing the asynchronous operation, returning the updated meal plan item.</returns>

        public async Task<PlanMeal?> UpdateMeal(int planId, PlanMeal meal)
        {
            try
            {
                var existingMeal = await GetMeal(meal.PlanItemId);
                if (existingMeal == null) return null;

                existingMeal.Name = meal.Name;
                existingMeal.Description = meal.Description;
                await _context.SaveChangesAsync();

                return existingMeal;
            }
            catch(Exception ex)
            {
                printMessage($"Error in UpdateMeal of PlansInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Deletes a meal plan item based on the provided meal ID.
        /// </summary>
        /// <param name="mealId">The ID of the meal plan item to be deleted.</param>
        /// <returns>A task representing the asynchronous operation, returning the deleted meal plan item.</returns>

        public async Task<PlanMeal?> DeleteMeal(int mealId)
        {
            try
            {
                var meal = await GetMeal(mealId);
                if (meal == null) return null;

                _context.PlanItems.Remove(meal);
                await _context.SaveChangesAsync();

                return meal;
            }
            catch(Exception ex)
            {
                printMessage($"Error in DeleteMeal of PlansInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Retrieves an exercise plan item based on the provided exercise ID.
        /// </summary>
        /// <param name="exerciseId">The ID of the exercise plan item.</param>
        /// <returns>A task representing the asynchronous operation, returning the exercise plan item.</returns>

        public async Task<PlanExercise?> GetExercise(int exerciseId)
        {
            try
            {
                var exercise = await _context.PlanItems.OfType<PlanExercise>()
                    .Include(m => m.Exercise).ThenInclude(e => e.Image)
                    .Include(m => m.Plan).ThenInclude(p => p.Client)
                    .SingleOrDefaultAsync(e => e.PlanItemId == exerciseId);

                return exercise;
            }
            catch(Exception ex)
            {
                printMessage($"Error in GetExercise of PlansInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Creates a new exercise plan item within the specified plan.
        /// </summary>
        /// <param name="planId">The ID of the plan to which the exercise will be added.</param>
        /// <param name="exercise">The exercise plan item to be created.</param>
        /// <returns>A task representing the asynchronous operation, returning the created exercise plan item.</returns>

        public async Task<PlanExercise?> CreateExercise(int planId, PlanExercise exercise)
        {
            try
            {
                var plan = await GetPlan(planId);
                plan.Items.Add(exercise);
                await _context.SaveChangesAsync();
                return exercise;
            }
            catch(Exception ex)
            {
                printMessage($"Error in CreateExercise of PlansInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Updates an existing exercise plan item within the specified plan.
        /// </summary>
        /// <param name="planId">The ID of the plan containing the exercise to be updated.</param>
        /// <param name="exercise">The updated exercise plan item.</param>
        /// <returns>A task representing the asynchronous operation, returning the updated exercise plan item.</returns>

        public async Task<PlanExercise?> UpdateExercise(int planId, PlanExercise exercise)
        {
            try
            {
                var existingExercise = await GetExercise(exercise.PlanItemId);
                if (existingExercise == null) return null;

                existingExercise.Name = exercise.Name;
                existingExercise.Description = exercise.Description;
                existingExercise.ExerciseId = exercise.ExerciseId;

                await _context.SaveChangesAsync();

                return existingExercise;
            }
            catch(Exception ex)
            {
                printMessage($"Error in UpdateExercise of PlansInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Deletes an exercise plan item based on the provided exercise ID.
        /// </summary>
        /// <param name="exerciseId">The ID of the exercise plan item to be deleted.</param>
        /// <returns>A task representing the asynchronous operation, returning the deleted exercise plan item.</returns>

        public async Task<PlanExercise?> DeleteExercise(int exerciseId)
        {
            try
            {
                var exercise = await GetExercise(exerciseId);
                if (exercise == null) return null;

                _context.PlanItems.Remove(exercise);
                await _context.SaveChangesAsync();

                return exercise;
            }
            catch(Exception ex)
            {
                printMessage($"Error in DeleteExercise of PlansInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Retrieves a plan based on the provided plan ID.
        /// </summary>
        /// <param name="planId">The ID of the plan.</param>
        /// <returns>A task representing the asynchronous operation, returning the plan.</returns>

        public async Task<Plan?> GetPlan(int planId)
        {
            try
            {
                var plan = await _context.Plans
                    .Include(p => p.Client)
                    .Include(p => p.HubImage)
                    .Include(p => p.Items)
                    .SingleOrDefaultAsync(i => i.PlanId == planId);

                return plan;
            }
            catch(Exception ex)
            {
                printMessage($"Error in GetPlan of PlansInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Creates a new plan for a specified user.
        /// </summary>
        /// <param name="userId">The ID of the user for whom the plan will be created.</param>
        /// <param name="plan">The plan to be created.</param>
        /// <returns>A task representing the asynchronous operation, returning the created plan.</returns>

        public async Task<Plan?> CreatePlan(int userId, Plan plan)
        {
            try
            {
                var client = await _context.Client.SingleOrDefaultAsync(c => c.UserId == userId);
                plan.Client = client;
                await _context.Plans.AddAsync(plan);
                await _context.SaveChangesAsync();
                return plan;
            }
            catch(Exception ex)
            {
                printMessage($"Error in CreatePlan of PlansInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Updates an existing plan for a specified user.
        /// </summary>
        /// <param name="userId">The ID of the user for whom the plan will be updated.</param>
        /// <param name="plan">The updated plan.</param>
        /// <returns>A task representing the asynchronous operation, returning the updated plan.</returns>

        public async Task<Plan?> UpdatePlan(int userId, Plan plan)
        {
            try
            {
                var existingPlan = await GetPlan(plan.PlanId);
                if (existingPlan == null) return null;

                existingPlan.Title = plan.Title;
                existingPlan.Description = plan.Description;
                await _context.SaveChangesAsync();

                return existingPlan;
            }
            catch(Exception ex)
            {
                printMessage($"Error in UpdatePlan of PlansInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Deletes a plan based on the provided plan ID.
        /// </summary>
        /// <param name="planId">The ID of the plan to be deleted.</param>
        /// <returns>A task representing the asynchronous operation, returning the deleted plan.</returns>

        public async Task<Plan?> DeletePlan(int planId)
        {
            try
            {
                var plan = await GetPlan(planId);
                if (plan == null) return null;

                _context.Plans.Remove(plan);
                await _context.SaveChangesAsync();

                return plan;
            }
            catch(Exception ex)
            {
                printMessage($"Error in DeletePlan of PlansInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Retrieves a list of meal plans associated with the provided client ID.
        /// </summary>
        /// <param name="clientId">The ID of the client.</param>
        /// <returns>A task representing the asynchronous operation, returning a list of meal plans.</returns>

        public async Task<List<Plan>> GetMealPlans(int clientId)
        {
            try
            {
                var plans = await _context.Plans.Where(p => p.PlanType == PlanType.NUTRITION && p.ClientId == clientId).ToListAsync();
                return plans;
            }
            catch(Exception ex)
            {
                printMessage($"Error in GetMealPlans of PlansInfo: {ex.Message}");
                return new List<Plan>();
            }
        }
        /// <summary>
        /// Retrieves a list of exercise plans associated with the provided client ID.
        /// </summary>
        /// <param name="clientId">The ID of the client.</param>
        /// <returns>A task representing the asynchronous operation, returning a list of exercise plans.</returns>

        public async Task<List<Plan>> GetExercisePlans(int clientId)
        {
            try
            {
                var plans = await _context.Plans.Where(p => p.PlanType == PlanType.EXERCISE && p.ClientId == clientId).ToListAsync();
                return plans;
            }
            catch(Exception ex)
            {
                printMessage($"Error in GetExercisePlans of PlansInfo: {ex.Message}");
                return new List<Plan>();
            }
        }

        /// <summary>
        /// Creates a new image for a meal plan item.
        /// </summary>
        /// <param name="mealId">The ID of the meal plan item.</param>
        /// <returns>A task representing the asynchronous operation, returning the created image.</returns>

        public async Task<HubImage?> CreateMealImage(int mealId)
        {
            try{
                var meal = await _context.PlanItems.OfType<PlanMeal>().FirstOrDefaultAsync(m => m.PlanItemId == mealId);
                if (meal == null) return null;


                var name = "image_meal" + meal.PlanId + "" + meal.PlanItemId;
                if (meal.HubImage == null)
                {
                    var img = new HubImage
                    {
                        Description = "No description",
                        Name = name,
                        Path = ""
                    };
                    meal.HubImage = img;
                }
                else
                {
                    meal.HubImage.Name = name;
                }

                await _context.SaveChangesAsync();

                return meal.HubImage;
            }
            catch(Exception ex)
            {
                printMessage($"Error in CreateMealImage of PlansInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Creates a new image for a plan.
        /// </summary>
        /// <param name="planId">The ID of the plan.</param>
        /// <returns>A task representing the asynchronous operation, returning the created image.</returns>

        public async Task<HubImage?> CreatePlanImage(int planId)
        {
            try{
                var plan = await _context.Plans.FirstOrDefaultAsync(m => m.PlanId == planId);
                if (plan == null) return null;


                var name = "image_plan" + plan.ClientId + "" + plan.PlanId;
                if (plan.HubImage == null)
                {
                    var img = new HubImage
                    {
                        Description = "No description",
                        Name = name,
                        Path = ""
                    };
                    plan.HubImage = img;
                }
                else
                {
                    plan.HubImage.Name = name;
                }

                await _context.SaveChangesAsync();

                return plan.HubImage;
            }
            catch(Exception ex)
            {
                printMessage($"Error in CreatePlanImage of PlansInfo: {ex.Message}");
                return null;
            }
        }/// <summary>
/// Updates the path of an image.
/// </summary>
/// <param name="imageName">The name of the image.</param>
/// <param name="newPath">The new path for the image.</param>
/// <returns>A task representing the asynchronous operation, returning the updated image.</returns>

        public async Task<HubImage?> UpdatePathImage(string imageName, string newPath)
        {
            try
            {
                var img = await _context.Images.FirstOrDefaultAsync(i => i.Name == imageName);
                if (img == null) return null;

                img.Path = newPath;

                await _context.SaveChangesAsync();

                return img;
            }
            catch(Exception ex)
            {
                printMessage($"Error in UpdatePathImage of PlansInfo: {ex.Message}");
                return null;
            }
        }/// <summary>
/// Removes an image based on the provided image name.
/// </summary>
/// <param name="imageName">The name of the image to be removed.</param>
/// <returns>A task representing the asynchronous operation, returning the removed image.</returns>

        public async Task<HubImage?> RemoveImage(string imageName)
        {
            try
            {
                var img = await _context.Images.FirstOrDefaultAsync(i => i.Name == imageName);
                if (img == null) return null;

                _context.Images.Remove(img);
                await _context.SaveChangesAsync();

                return img;
            }
            catch(Exception ex)
            {
                printMessage($"Error in RemoveImage of PlansInfo: {ex.Message}");
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
