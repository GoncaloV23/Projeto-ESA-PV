using EasyFitHub.Models.Gym;
using EasyFitHub.Models.Inventory;
using EasyFitHub.Models.Miscalenous;
using EasyFitHub.Models.Profile;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Reflection.PortableExecutable;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EasyFitHub.Data
{
    /// <summary>
    /// AUTHOR: Rui Barroso
    /// Usado pra efetuar operações CRUD no Inventaário dos ginásios
    /// </summary>
    public class InventoryInfo
    {
        private readonly EasyFitHubContext _context;
        private readonly ILogger? _logger;
        public InventoryInfo(EasyFitHubContext context, ILogger? logger = null)
        {
            _context = context;
            _logger = logger;
        }
        /// <summary>
        /// Retrieves the list of gym machines for a given gym ID.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <returns>A task representing the asynchronous operation, returning a list of gym machines.</returns>
        public async Task<List<GymMachine>> GetMachines(int gymId) 
        {
            try
            {
                return await  _context.Gym
                    .Where(g => g.Id == gymId)
                    .SelectMany(g => g.Machines)
                    .Include(m => m.Image)
                    .ToListAsync();
            }
            catch(Exception ex)
            {
                printMessage($"Error in GetMachines of InventoryInfo: {ex.Message}");
                return new List<GymMachine>();
            }
        }/// <summary>
/// Retrieves a gym machine by its ID.
/// </summary>
/// <param name="machineId">The ID of the gym machine.</param>
/// <returns>A task representing the asynchronous operation, returning the gym machine.</returns>
        public async Task<GymMachine?> GetMachine(int machineId)
        {
            try
            {
                var machine = await _context.GymMachines.Include(i => i.Exercise).ThenInclude(e => e.Image).Include(i => i.Image).SingleOrDefaultAsync(i => i.MachineId == machineId);

                return machine;
            }
            catch(Exception ex)
            {
                printMessage($"Error in GetMachine of InventoryInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Adds a gym machine to the gym's inventory.
        /// </summary>
        /// <param name="gym">The gym to which the machine will be added.</param>
        /// <param name="machine">The gym machine to add.</param>
        /// <returns>A task representing the asynchronous operation, returning the added gym machine.</returns>
        public async Task<GymMachine?> AddMachine(Gym gym, GymMachine machine) 
        {
            try
            {
                var updatedGym = await _context.Gym.FindAsync(gym.Id);

                if (updatedGym == null) return null;

                updatedGym.Machines.Add(machine);
                var result = await _context.GymMachines.AddAsync(machine);

                if (result.State != EntityState.Added)
                    return null;

                await _context.SaveChangesAsync();

                return result.Entity;
            }
            catch(Exception ex)
            {
                printMessage($"Error in AddMachine of InventoryInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Updates information about a gym machine.
        /// </summary>
        /// <param name="gym">The gym containing the machine.</param>
        /// <param name="machine">The updated information of the gym machine.</param>
        /// <returns>A task representing the asynchronous operation, returning the updated gym machine.</returns>
        public async Task<GymMachine?> UpdateMachine(Gym gym, GymMachine machine) 
        {
            try
            {
                var updatedGym = await _context.Gym.FindAsync(gym.Id);

                if (updatedGym == null)
                    return null;

                var existingMachine = updatedGym.Machines.Single(m => m.MachineId == machine.MachineId);

                if (existingMachine == null)
                    return null;

                
                existingMachine.Name = machine.Name;
                existingMachine.Exercise = machine.Exercise;
                existingMachine.Quantity = machine.Quantity;
                existingMachine.Description = machine.Description;
                existingMachine.Image = machine.Image;

                _context.SaveChanges();

                return existingMachine;
            }
            catch(Exception ex)
            {
                printMessage($"Error in UpdateMachine of InventoryInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Removes a gym machine from the inventory.
        /// </summary>
        /// <param name="machineId">The ID of the gym machine to remove.</param>
        /// <returns>A task representing the asynchronous operation, returning the removed gym machine.</returns>
        public async Task<GymMachine?> RemoveMachine(int machineId) 
        {
            try
            {
                var machineToRemove = await _context.GymMachines.FindAsync(machineId);

                if (machineToRemove == null)
                    return null;

                var result = _context.GymMachines.Remove(machineToRemove);

                if (result.State != EntityState.Deleted) return null;

                await _context.SaveChangesAsync();

                return result.Entity;
            }
            catch(Exception ex)
            {
                printMessage($"Error in RemoveMachine of InventoryInfo: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Retrieves the list of exercises associated with a gym machine.
        /// </summary>
        /// <param name="machineId">The ID of the gym machine.</param>
        /// <returns>A task representing the asynchronous operation, returning a list of exercises.</returns>

        public async Task<List<Exercise>> GetExercises(int machineId)
        {
            try
            {
                return await _context.GymMachines
                    .Where(m => m.MachineId == machineId)
                    .SelectMany(m => m.Exercise)
                    .Include(e => e.Image)
                    .ToListAsync();
            }
            catch(Exception ex)
            {
                printMessage($"Error in GetExercises of InventoryInfo: {ex.Message}");
                return new List<Exercise>();
            }
        }
        /// <summary>
        /// Retrieves an exercise by its ID.
        /// </summary>
        /// <param name="exerciseId">The ID of the exercise.</param>
        /// <returns>A task representing the asynchronous operation, returning the exercise.</returns>
        public async Task<Exercise?> GetExercise(int exerciseId)
        {
            try
            {
                var exercise = await _context.Exercises.Include(i => i.Image).SingleOrDefaultAsync(i => i.ExerciseId == exerciseId);

                return exercise;
            }
            catch(Exception ex)
            {
                printMessage($"Error in GetExercise of InventoryInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Adds an exercise to a gym machine.
        /// </summary>
        /// <param name="machine">The gym machine to which the exercise will be added.</param>
        /// <param name="exercise">The exercise to add.</param>
        /// <returns>A task representing the asynchronous operation, returning the added exercise.</returns>
        public async Task<Exercise?> AddExercise(GymMachine machine, Exercise exercise)
        {
            try
            {
                var updatedMachine = await _context.GymMachines.FindAsync(machine.MachineId);

                if (updatedMachine == null) return null;

                updatedMachine.Exercise.Add(exercise);
                var result = await _context.Exercises.AddAsync(exercise);

                if (result.State != EntityState.Added)
                    return null;

                await _context.SaveChangesAsync();

                return result.Entity;
            }
            catch(Exception ex)
            {
                printMessage($"Error in AddExercise of InventoryInfo: {ex.Message}");
                return null;
            }
        }/// <summary>
/// Updates information about an exercise.
/// </summary>
/// <param name="machine">The gym machine containing the exercise.</param>
/// <param name="exercise">The updated information of the exercise.</param>
/// <returns>A task representing the asynchronous operation, returning the updated exercise.</returns>

        public async Task<Exercise?> UpdateExercise(GymMachine machine, Exercise exercise)
        {
            try
            {
                var updatedMachine = await _context.GymMachines.FindAsync(machine.MachineId);

                if (updatedMachine == null)
                    return null;

                var existingExercise = updatedMachine.Exercise.Single(e => e.ExerciseId == exercise.ExerciseId);

                if (existingExercise == null)
                    return null;


                existingExercise.Name = exercise.Name;
                existingExercise.Description = exercise.Description;
                existingExercise.Image = exercise.Image;

                _context.SaveChanges();

                return existingExercise;
            }
            catch(Exception ex)
            {
                printMessage($"Error in UpdateExercise of InventoryInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Removes an exercise from the inventory.
        /// </summary>
        /// <param name="exerciseId">The ID of the exercise to remove.</param>
        /// <returns>A task representing the asynchronous operation, returning the removed exercise.</returns>
        public async Task<Exercise?> RemoveExercise(int exerciseId)
        {
            try
            {
                var exerciseToRemove = await _context.Exercises.FindAsync(exerciseId);

                if (exerciseToRemove == null)
                    return null;

                var result = _context.Exercises.Remove(exerciseToRemove);

                if (result.State != EntityState.Deleted) return null;

                await _context.SaveChangesAsync();

                return result.Entity;
            }
            catch(Exception ex)
            {
                printMessage($"Error in RemoveExercise of InventoryInfo: {ex.Message}");
                return null;
            }
        }


        /// <summary>
        /// Retrieves the list of items associated with a gym.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <returns>A task representing the asynchronous operation, returning a list of items.</returns>
        public async Task<List<Item>> GetItems(int gymId)
        {
            try
            {
                return await _context.Gym
                    .Where(g => g.Id == gymId)
                    .SelectMany(g => g.Items)
                    .Include(i => i.Image)
                    .ToListAsync();
            }
            catch(Exception ex)
            {
                printMessage($"Error in GetItems of InventoryInfo: {ex.Message}");
                return new List<Item>();
            }
        }
        /// <summary>
        /// Retrieves an item by its ID.
        /// </summary>
        /// <param name="itemId">The ID of the item.</param>
        /// <returns>A task representing the asynchronous operation, returning the item.</returns>
        public async Task<Item?> GetItem(int itemId)
        {
            try
            {
                var item = await _context.Items.Include(i => i.Image).SingleOrDefaultAsync(i => i.ItemId == itemId);
                    
                return item;
            }
            catch(Exception ex)
            {
                printMessage($"Error in GetItem of InventoryInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Adds an item to a gym's inventory.
        /// </summary>
        /// <param name="gym">The gym to which the item will be added.</param>
        /// <param name="item">The item to add.</param>
        /// <returns>A task representing the asynchronous operation, returning the added item.</returns>
        public async Task<Item?> AddItem(Gym gym, Item item)
        {
            try
            {
                var updatedGym = await _context.Gym.FindAsync(gym.Id);

                if (updatedGym == null) return null;

                updatedGym.Items.Add(item);
                var result = await _context.Items.AddAsync(item);

                if (result.State != EntityState.Added)
                    return null;

                await _context.SaveChangesAsync();

                return result.Entity;
            }
            catch(Exception ex)
            {
                printMessage($"Error in AddItem of InventoryInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Updates information about an item.
        /// </summary>
        /// <param name="gym">The gym containing the item.</param>
        /// <param name="item">The updated information of the item.</param>
        /// <returns>A task representing the asynchronous operation, returning the updated item.</returns>
        public async Task<Item?> UpdateItem(Gym gym, Item item)
        {
            try
            {
                var updatedGym = await _context.Gym.FindAsync(gym.Id);

                if (updatedGym == null)
                    return null;

                var existingItem = updatedGym.Items.Single(i => i.ItemId == item.ItemId);

                if (existingItem == null)
                    return null;


                existingItem.Name = item.Name;
                existingItem.Price = item.Price;
                existingItem.Quantity = item.Quantity;
                existingItem.Description = item.Description;
                existingItem.Image = item.Image;

                _context.SaveChanges();

                return existingItem;
            }
            catch(Exception ex)
            {
                printMessage($"Error in UpdateItem of InventoryInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Removes an item from the inventory.
        /// </summary>
        /// <param name="itemId">The ID of the item to remove.</param>
        /// <returns>A task representing the asynchronous operation, returning the removed item.</returns>
        public async Task<Item?> RemoveItem(int itemId)
        {
            try
            {
                var itemToRemove = await _context.Items.FindAsync(itemId);

                if (itemToRemove == null)
                    return null;

                var result = _context.Items.Remove(itemToRemove);

                if (result.State != EntityState.Deleted) return null;

                await _context.SaveChangesAsync();

                return result.Entity;
            }
            catch(Exception ex)
            {
                printMessage($"Error in RemoveItem of InventoryInfo: {ex.Message}");
                return null;
            }
        }



        /// <summary>
        /// Creates an image for a specified entity type.
        /// </summary>
        /// <param name="id">The ID of the entity.</param>
        /// <param name="type">The type of the entity (Exercise, Item, or GymMachine).</param>
        /// <returns>A task representing the asynchronous operation, returning the created image.</returns>
        public async Task<HubImage?> CreateImage(int id, string type)
        {
            switch (type)
            {
                case nameof(Exercise):
                    return await CreateExerciseImage(id);
                case nameof(Item):
                    return await CreateItemImage(id);
                case nameof(GymMachine):
                    return await CreateMachineImage(id);
                default: 
                    return null;
            }
        }
        /// <summary>
        /// Creates an image for an exercise.
        /// </summary>
        /// <param name="exerciseId">The ID of the exercise.</param>
        /// <returns>A task representing the asynchronous operation, returning the created image.</returns>
        public async Task<HubImage?> CreateExerciseImage(int exerciseId) 
        {
            try
            {
                var exercise = await _context.Exercises.FirstOrDefaultAsync(e => e.ExerciseId == exerciseId);
                if (exercise == null) return null;

                var name = "image_exercise" + exercise.MachineId + "" + exercise.ExerciseId;
                if (exercise.Image == null)
                {
                    var img = new HubImage
                    {
                        Description = "No description",
                        Name = name,
                        Path = ""
                    };
                    exercise.Image = img;
                }
                else
                {
                    exercise.Image.Name = name;
                }

                await _context.SaveChangesAsync();

                return exercise.Image;
            }
            catch(Exception ex)
            {
                printMessage($"Error in CreateExerciseImage of InventoryInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Creates an image for an item.
        /// </summary>
        /// <param name="itemId">The ID of the item.</param>
        /// <returns>A task representing the asynchronous operation, returning the created image.</returns>
        public async Task<HubImage?> CreateItemImage(int itemId)
        {
            try
            {
                var item = await _context.Items.FirstOrDefaultAsync(i => i.ItemId == itemId);
                if (item == null) return null;

                
                var name = "image_item" + item.GymId + "" + item.ItemId;
                if (item.Image == null)
                {
                    var img = new HubImage
                    {
                        Description = "No description",
                        Name = name,
                        Path = ""
                    };
                    item.Image = img;
                }
                else
                {
                    item.Image.Name = name;
                }

                await _context.SaveChangesAsync();

                return item.Image;
            }
            catch(Exception ex)
            {
                printMessage($"Error in CreateItemImage of InventoryInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Creates an image for a gym machine.
        /// </summary>
        /// <param name="machineId">The ID of the gym machine.</param>
        /// <returns>A task representing the asynchronous operation, returning the created image.</returns>
        public async Task<HubImage?> CreateMachineImage(int machineId)
        {
            try
            {
                var machine = await _context.GymMachines.FirstOrDefaultAsync(m => m.MachineId == machineId);
                if (machine == null) return null;

                
                var name = "image_machine" + machine.GymId + "" + machine.MachineId;
                if (machine.Image == null)
                {
                    var img = new HubImage
                    {
                        Description = "No description",
                        Name = name,
                        Path = ""
                    };
                    machine.Image = img;
                }
                else
                {
                    machine.Image.Name = name;
                }

                await _context.SaveChangesAsync();

                return machine.Image;
            }
            catch(Exception ex)
            {
                printMessage($"Error in CreateMachineImage of InventoryInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Updates the path of an existing image.
        /// </summary>
        /// <param name="imageName">The name of the image to update.</param>
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
                printMessage($"Error in UpdatePathImage of InventoryInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Removes an image from the database.
        /// </summary>
        /// <param name="imageName">The name of the image to remove.</param>
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
                printMessage($"Error in RemoveImage of InventoryInfo: {ex.Message}");
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
