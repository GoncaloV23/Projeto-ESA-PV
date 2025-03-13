using EasyFitHub.Data;
using EasyFitHub.Models.Inventory;
using EasyFitHub.Models.Profile;
using EasyFitHub.Services;
using Microsoft.AspNetCore.Mvc;
using NuGet.Configuration;
using System.Reflection.PortableExecutable;

namespace EasyFitHub.Controllers
{
    /// <summary>
    /// AUTHOR: Rui Barroso
    /// 
    /// Controlador responsável pela gestão de Inventário de um ginásio
    /// O controlador gere Máquinas, exercícios e a loja
    /// </summary>
    public class InventoryController : Controller
    {
        private readonly AuthorizationService _authorizationService;
        private readonly ILogger<InventoryController> _logger;
        private readonly InventoryInfo _inventoryInfo;
        private readonly FirebaseService _firebaseService;

        public InventoryController(ILogger<InventoryController> logger, EasyFitHubContext context, AuthorizationService authorizationService, FirebaseService firebaseService) 
        { 
            _inventoryInfo = new InventoryInfo(context, logger);
            _logger = logger;            
            _authorizationService = authorizationService;
            _firebaseService = firebaseService;
        }
        /// <summary>
        /// Displays the list of items available in the gym.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <returns>Returns the appropriate view based on authorization and edit rights.</returns>
        //Items
        public async Task<IActionResult> Items(int gymId)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);
            
            if (result.Gym == null || !result.IsAuthorizedToRead) return View("NotAuthorized");


            var items = await _inventoryInfo.GetItems(gymId);

            if (result.IsAuthorizedToEdit) 
            {
                TempData["GymId"] = gymId;
                return View("ItemsEdit", items);
            } else
            {
                TempData["GymId"] = gymId;
                return View(items);
            }

        }
        /// <summary>
        /// Displays details of a specific item.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="itemId">The ID of the item.</param>
        /// <returns>Returns the appropriate view based on authorization and edit rights.</returns>
        public async Task<IActionResult> Item(int gymId, int itemId)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || !result.IsAuthorizedToRead) return View("NotAuthorized");

            var item = await _inventoryInfo.GetItem(itemId);

            if (result.IsAuthorizedToEdit)
            {
                TempData["GymId"] = gymId;
                return View("ItemEdit", item);
            }
            else
            {
                TempData["GymId"] = gymId;
                return View(item);
            }
        }
        /// <summary>
        /// Displays the form to create a new item.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <returns>Returns the appropriate view based on authorization and edit rights.</returns>
        public async Task<IActionResult> CreateItem(int gymId)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || !result.IsAuthorizedToRead) return View("NotAuthorized");
            

            if (result.IsAuthorizedToEdit)
            {
                TempData["GymId"] = gymId;
                return View();
            }
            else
            {
                return RedirectToAction("Items", new { gymId = gymId });
            }
        }
        /// <summary>
        /// Adds a new item to the gym's inventory.
        /// </summary>
        /// <param name="file">The image file of the item.</param>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="item">The item details.</param>
        /// <returns>Returns the appropriate action based on the success of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> AddItem(IFormFile file, int gymId, [Bind("Name,Description,Quantity,Price")] Item item)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || !result.IsAuthorizedToEdit) return View("NotAuthorized");

            var addedItem = await _inventoryInfo.AddItem(result.Gym, item);

            if (addedItem != null && file != null && file.Length > 0)
                return await UploadFile( gymId, addedItem.ItemId, nameof(Item),file );

            return RedirectToAction("Items", new {gymId = gymId});
        }
        /// <summary>
        /// Updates an existing item in the gym's inventory.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="item">The updated item details.</param>
        /// <returns>Returns the appropriate action based on the success of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateItem(int gymId, Item item)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || !result.IsAuthorizedToEdit) return View("NotAuthorized");

            var updatedItem = await _inventoryInfo.UpdateItem(result.Gym, item);

            return RedirectToAction("Item", new { gymId = gymId, itemId = item.ItemId });
        }/// <summary>
/// Deletes an item from the gym's inventory.
/// </summary>
/// <param name="gymId">The ID of the gym.</param>
/// <param name="itemId">The ID of the item to delete.</param>
/// <returns>Returns the appropriate action based on the success of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteItem(int gymId, int itemId)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || !result.IsAuthorizedToEdit) return View("NotAuthorized");

            var removedItem = await _inventoryInfo.RemoveItem(itemId);

            return RedirectToAction("Items", new { gymId = gymId });
        }
        /// <summary>
        /// Displays the list of machines available in the gym.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <returns>Returns the appropriate view based on authorization and edit rights.</returns>
        //Machines
        public async Task<IActionResult> Machines(int gymId)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || !result.IsAuthorizedToRead) return View("NotAuthorized");

            var machines = await _inventoryInfo.GetMachines(gymId);

            if (result.IsAuthorizedToEdit)
            {
                TempData["GymId"] = gymId;
                return View("MachinesEdit", machines);
            }
            else
            {
                TempData["GymId"] = gymId;
                return View(machines);
            }
        }
        /// <summary>
        /// Displays details of a specific machine.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="machineId">The ID of the machine.</param>
        /// <returns>Returns the appropriate view based on authorization and edit rights.</returns>
        public async Task<IActionResult> Machine(int gymId, int machineId)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || !result.IsAuthorizedToRead) return View("NotAuthorized");

            var machine = await _inventoryInfo.GetMachine(machineId);

            if (result.IsAuthorizedToEdit)
            {
                TempData["GymId"] = gymId;
                return View("MachineEdit", machine);
            }
            else
            {
                TempData["GymId"] = gymId;
                return View(machine);
            }
        }
        /// <summary>
        /// Displays the form to create a new machine.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <returns>Returns the appropriate view based on authorization and edit rights.</returns>
        public async Task<IActionResult> CreateMachine(int gymId)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || !result.IsAuthorizedToRead) return View("NotAuthorized");


            if (result.IsAuthorizedToEdit)
            {
                TempData["GymId"] = gymId;
                return View();
            }
            else
            {
                return RedirectToAction("Machines", new { gymId = gymId });
            }
        }
        /// <summary>
        /// Adds a new machine to the gym's inventory.
        /// </summary>
        /// <param name="file">The image file of the machine.</param>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="machine">The machine details.</param>
        /// <returns>Returns the appropriate action based on the success of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> AddMachine(IFormFile file, int gymId, GymMachine machine)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || !result.IsAuthorizedToEdit) return View("NotAuthorized");

            var addedMachine = await _inventoryInfo.AddMachine(result.Gym, machine);

            if (addedMachine != null && file != null && file.Length > 0)
                return await UploadFile(gymId, addedMachine.MachineId, nameof(GymMachine), file);

            return RedirectToAction("Machines", new { gymId = gymId });
        }
        /// <summary>
        /// Updates an existing machine in the gym's inventory.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="machine">The updated machine details.</param>
        /// <returns>Returns the appropriate action based on the success of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateMachine(int gymId, GymMachine machine)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || !result.IsAuthorizedToEdit) return View("NotAuthorized");

            var updatedMachine = await _inventoryInfo.UpdateMachine(result.Gym, machine);

            return RedirectToAction("Machine", new { machineId = machine.MachineId, gymId = gymId });
        }
        /// <summary>
        /// Deletes a machine from the gym's inventory.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="machineId">The ID of the machine to delete.</param>
        /// <returns>Returns the appropriate action based on the success of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteMachine(int gymId, int machineId)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || !result.IsAuthorizedToEdit) return View("NotAuthorized");

            var removedMachine = await _inventoryInfo.RemoveMachine(machineId);

            return RedirectToAction("Machines", new { gymId = gymId });
        }

        /// <summary>
        /// Displays details of a specific exercise.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="machineId">The ID of the machine.</param>
        /// <param name="exerciseId">The ID of the exercise.</param>
        /// <returns>Returns the appropriate view based on authorization and edit rights.</returns>
        //Exercises
        public async Task<IActionResult> Exercise(int gymId, int machineId, int exerciseId)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || !result.IsAuthorizedToRead) return View("NotAuthorized");

            var exercise = await _inventoryInfo.GetExercise(exerciseId);

            if (result.IsAuthorizedToEdit)
            {
                TempData["GymId"] = gymId;
                TempData["MachineId"] = machineId;
                return View("UpdateExercise", exercise);
            }
            else
            {
                TempData["GymId"] = gymId;
                TempData["MachineId"] = machineId;
                return View(exercise);
            }


        }
        /// <summary>
         /// Displays a form to create a exercise.
         /// </summary>
         /// <param name="gymId">The ID of the gym.</param>
         /// <param name="machineId">The ID of the machine.</param>
         /// <returns>Returns the appropriate view based on authorization and edit rights.</returns>
        //Exercises
        public async Task<IActionResult> CreateExercise(int gymId, int machineId)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || !result.IsAuthorizedToRead) return View("NotAuthorized");

            if (result.IsAuthorizedToEdit)
            {
                var exercise = new Exercise
                {
                    MachineId = machineId
                };

                TempData["GymId"] = gymId;
                TempData["MachineId"] = machineId;
                return View(exercise);
            }

            return RedirectToAction("Machine", new { machineId = machineId, gymId = gymId });
        }
        /// <summary>
        /// Adds a new exercise to the specified machine in the gym's inventory.
        /// </summary>
        /// <param name="file">The image file of the exercise.</param>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="machineId">The ID of the machine.</param>
        /// <param name="exercise">The exercise details.</param>
        /// <returns>Returns the appropriate action based on the success of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> AddExercise(IFormFile file, int gymId, int machineId, Exercise exercise)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || !result.IsAuthorizedToEdit) return View("NotAuthorized");

            var machine = await _inventoryInfo.GetMachine(machineId);
            var addedExercise = await _inventoryInfo.AddExercise(machine, exercise);

            if (addedExercise != null && file != null && file.Length > 0)
                return await UploadFile(gymId, addedExercise.MachineId, nameof(EasyFitHub.Models.Inventory.Exercise), file);

            return RedirectToAction("Machine", new { machineId = machineId, gymId = gymId });
        }
        /// <summary>
        /// Updates an existing exercise in the specified machine in the gym's inventory.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="machineId">The ID of the machine.</param>
        /// <param name="exercise">The updated exercise details.</param>
        /// <returns>Returns the appropriate action based on the success of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateExercise(int gymId, int machineId, Exercise exercise)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || !result.IsAuthorizedToEdit) return View("NotAuthorized");

            var machine = await _inventoryInfo.GetMachine(machineId);
            var removedExercise = await _inventoryInfo.UpdateExercise(machine, exercise);

            return RedirectToAction("Exercise", new { gymId = gymId, machineId = machineId, exerciseId = exercise.ExerciseId});
        }
        /// <summary>
        /// Deletes an exercise from the specified machine in the gym's inventory.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <param name="machineId">The ID of the machine.</param>
        /// <param name="exerciseId">The ID of the exercise to delete.</param>
        /// <returns>Returns the appropriate action based on the success of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteExercise(int gymId, int machineId, int exerciseId)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || !result.IsAuthorizedToEdit) return View("NotAuthorized");

            //var machine = await _inventoryInfo.GetMachine(machineId);
            var removedExercise = await _inventoryInfo.RemoveExercise(exerciseId);

            return RedirectToAction("Machine", new { machineId = machineId, gymId = gymId });
        }


        /// <summary>
        /// Uploads an image to Firebase Service
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="id">Item Id</param>
        /// <param name="itemType">Item type (Machine, Exercise or Item)</param>
        /// <param name="file">Image file</param>
        /// <returns>redirects to the corresponding View to visualize the image change</returns>

        [HttpPost]
        public async Task<IActionResult> UploadFile(int gymId, int id, string itemType, IFormFile file)
        {
            if (file == null || file.Length <= 0) return RedirectToAction("Index", "Home");
            
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || !result.IsAuthorizedToEdit) return RedirectToAction("Index", "Home");

            switch (itemType)
            {
                case nameof(Exercise):
                    var exerciseImg = await _inventoryInfo.CreateExerciseImage(id);

                    using (var stream = file.OpenReadStream())
                    {
                        exerciseImg = await _firebaseService.CreateHubImage(exerciseImg, stream, _logger);
                    }

                    if (exerciseImg != null) await _inventoryInfo.UpdatePathImage(exerciseImg.Name, exerciseImg.Path);

                    var ex = await _inventoryInfo.GetExercise(id);
                    return RedirectToAction("Exercise", new { gymId = gymId, machineId = ex.MachineId, exerciseId = id });
                
                case nameof(Item):
                    var itemImg = await _inventoryInfo.CreateItemImage(id);
                    using (var stream = file.OpenReadStream())
                    {
                        itemImg = await _firebaseService.CreateHubImage(itemImg, stream, _logger);
                    }

                    if (itemImg != null) await _inventoryInfo.UpdatePathImage(itemImg.Name, itemImg.Path);
                    return RedirectToAction("Item", new { gymId = gymId, itemId = id });
                
                case nameof(GymMachine):
                    var machineImg = await _inventoryInfo.CreateMachineImage(id);
                    using (var stream = file.OpenReadStream())
                    {
                        machineImg = await _firebaseService.CreateHubImage(machineImg, stream, _logger);
                    }

                    if (machineImg != null) await _inventoryInfo.UpdatePathImage(machineImg.Name, machineImg.Path);
                    return RedirectToAction("Machine", new { gymId = gymId, machineId = id });
                
                default:
                    return null;
            }            
            

        }/// <summary>
        /// Removes an image from Firebase
        /// </summary>
        /// <param name="gymId">The gym from where the Image is from</param>
        /// <param name="imageName">The name of the Image</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteFile(int gymId, string imageName)
        {
            var result = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (result.Gym == null || !result.IsAuthorizedToEdit) return RedirectToAction("Index", "Home");

            var image = await _inventoryInfo.RemoveImage(imageName);

            var res = await _firebaseService.DeleteHubImage(image!);

            return Content("<script>window.location.reload();</script>");
        }


    }
}
