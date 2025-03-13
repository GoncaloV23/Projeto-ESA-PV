using EasyFitHub.Data;
using EasyFitHub.Models.Account;
using EasyFitHub.Models.Inventory;
using EasyFitHub.Models.Plan;
using EasyFitHub.Models.Profile;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFitHubTests
{
    internal class PlansTest
    {
        private PlansInfo plansInfo;
        Client defaultClient;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            DbContextOptions<EasyFitHubContext> options = new DbContextOptionsBuilder<EasyFitHubContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            TestContext context = new TestContext(options);
            plansInfo = new PlansInfo(context);

            var aux = context.Client.Add(
                new EasyFitHub.Models.Profile.Client
                {
                    User = new EasyFitHub.Models.Account.User
                    {
                        UserName = "test",
                        Email = "test@gmail.com",
                        Password = "test",
                        Name = "test",
                        Surname = "test",
                        BirthDate = new DateOnly(2002, 1, 6)
                    },
                    Biometrics = new EasyFitHub.Models.Profile.Biometrics { },
                    Data = new EasyFitHub.Models.Profile.ClientData { }
                }    
            );

            defaultClient = aux.Entity;


            context.Plans.AddRange(
                    new EasyFitHub.Models.Plan.Plan 
                    {
                        Title = "Plan1",
                        Description = "Nutrition1",
                        PlanType = PlanType.NUTRITION,
                        ClientId = defaultClient.ClientId
                    },
                    new EasyFitHub.Models.Plan.Plan
                    {
                        Title = "Plan2",
                        Description = "Nutrition2",
                        PlanType = PlanType.NUTRITION,
                        ClientId = defaultClient.ClientId
                    },
                    new EasyFitHub.Models.Plan.Plan
                    {
                        Title = "Plan3",
                        Description = "Exercise3",
                        PlanType = PlanType.EXERCISE,
                        ClientId = defaultClient.ClientId
                    }
            );

            context.SaveChanges();
        }

        [Test]
        public async Task PlanTest() 
        {
            var initialMeals = await plansInfo.GetMealPlans(defaultClient.ClientId);
            var initialExercises = await plansInfo.GetExercisePlans(defaultClient.ClientId);

            var plan = await plansInfo.CreatePlan(
                defaultClient.UserId,
                new EasyFitHub.Models.Plan.Plan
                {
                    Title = "Plan4",
                    Description = "Exercise4",
                    PlanType = PlanType.EXERCISE,
                }
             );
            var mealPlan = await plansInfo.CreatePlan(
                defaultClient.UserId,
                new EasyFitHub.Models.Plan.Plan
                {
                    Title = "Plan",
                    Description = "Meal",
                    PlanType = PlanType.NUTRITION,
                }
             );

            var afterMeals = await plansInfo.GetMealPlans(defaultClient.ClientId);
            var afterExercises = await plansInfo.GetExercisePlans(defaultClient.ClientId);

            var createdPlan = await plansInfo.GetPlan(plan.PlanId);
            string createdPlanTitle = createdPlan.Title;

            createdPlan.Title = "Plan5";
            await plansInfo.UpdatePlan(defaultClient.UserId, createdPlan);
            var updatedPlan =  await plansInfo.GetPlan(plan.PlanId);
            string updatedPlanTitle = updatedPlan.Title;

            await plansInfo.DeletePlan(plan.PlanId);
            var removedPlan = await plansInfo.GetPlan(plan.PlanId);

            await plansInfo.DeletePlan(mealPlan.PlanId);

            var finalMeals = await plansInfo.GetMealPlans(defaultClient.ClientId);
            var finalExercises = await plansInfo.GetExercisePlans(defaultClient.ClientId);

            Assert.That(plan, Is.Not.Null);
            Assert.That(createdPlanTitle, Is.EqualTo("Plan4"));
            Assert.That(updatedPlanTitle, Is.EqualTo("Plan5"));
            Assert.That(removedPlan, Is.Null);

            Assert.That(afterExercises.Count - initialExercises.Count, Is.EqualTo(1));
            Assert.That(finalExercises.Count - initialExercises.Count, Is.EqualTo(0));

            Assert.That(afterMeals.Count - initialMeals.Count, Is.EqualTo(1));
            Assert.That(finalMeals.Count - initialMeals.Count, Is.EqualTo(0));

            Assert.Pass();
        }

        [Test]
        public async Task MealTest() 
        {
            var mealPlan = (await plansInfo.GetMealPlans(defaultClient.ClientId)).FirstOrDefault();
            Assert.That(mealPlan, Is.Not.Null);

            var meal = await plansInfo.CreateMeal
            (
                mealPlan.PlanId,
                new PlanMeal
                {
                    Name = "Meal1",
                    Description = "Description 1",
                }
            );
            var createdMeal = await plansInfo.GetMeal(meal.PlanItemId);
            var Name = createdMeal.Name;

            meal.Name = "NewMealName";
            await plansInfo.UpdateMeal(meal.PlanId, meal);
            var updatedMeal = await plansInfo.GetMeal(meal.PlanItemId);
            var updatedName = updatedMeal.Name;

            await plansInfo.DeleteMeal(meal.PlanItemId);
            var removedMeal = await plansInfo.GetMeal(meal.PlanItemId);


            Assert.That(Name, Is.EqualTo("Meal1"));
            Assert.That(updatedName, Is.EqualTo("NewMealName"));
            Assert.That(removedMeal, Is.Null);

            Assert.Pass();
        }

        [Test]
        public async Task ExerciseTest() 
        {
            var exercisePlan = (await plansInfo.GetExercisePlans(defaultClient.ClientId)).FirstOrDefault();
            Assert.That(exercisePlan, Is.Not.Null);

            var exercise = await plansInfo.CreateExercise
            (
                exercisePlan.PlanId,
                new PlanExercise
                {
                    Name = "Exercise1",
                    Description = "Description 1",
                    Exercise = new Exercise 
                    { 
                        Name = "",
                        Description = "",
                        Machine = new GymMachine
                        {
                            Name="",
                            Description="",
                            Gym = new EasyFitHub.Models.Gym.Gym
                            { 
                                Name = "",
                                Description = "",
                            }
                        }
                    }
                }
            );
            var createdExercise = await plansInfo.GetExercise(exercise.PlanItemId);
            var Name = createdExercise.Name;

            exercise.Name = "NewExerciseName";
            await plansInfo.UpdateExercise(exercise.PlanId, exercise);
            var updatedExercise = await plansInfo.GetExercise(exercise.PlanItemId);
            var updatedName = updatedExercise.Name;

            await plansInfo.DeleteExercise(exercise.PlanItemId);
            var removedExercise = await plansInfo.GetExercise(exercise.PlanItemId);


            Assert.That(Name, Is.EqualTo("Exercise1"));
            Assert.That(updatedName, Is.EqualTo("NewExerciseName"));
            Assert.That(removedExercise, Is.Null);

            Assert.Pass();
        }
    }
}
