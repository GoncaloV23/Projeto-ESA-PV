using EasyFitHub.Data;
using EasyFitHub.Models.Account;
using EasyFitHub.Models.Gym;
using EasyFitHub.Models.Inventory;
using EasyFitHub.Models.Profile;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace EasyFitHubTests
{
    internal class InventoryInfoTest
    {
        Gym gym;

        private InventoryInfo inventoryInfo;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            DbContextOptions<EasyFitHubContext> options = new DbContextOptionsBuilder<EasyFitHubContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            TestContext context = new TestContext(options);
            inventoryInfo = new InventoryInfo(context);

            
            
            //Exercises
            IList<Exercise> exercises1 = new List<Exercise>
            {
                new Exercise
                {
                    Name = "Exercise1"
                },
                new Exercise
                {
                    Name = "Exercise2"
                }
            };
            IList<Exercise> exercises2 = new List<Exercise>
            {
                new Exercise
                {
                    Name = "Exercise3"
                }
            };

            //Machines
            IList<GymMachine> machines = new List<GymMachine>
            {   
                new GymMachine
                {
                    Name = "Machine1",
                    Quantity = 2,
                    Exercise = exercises1
                },
                new GymMachine
                {
                    Name = "Machine2",
                    Quantity = 5,
                    Exercise = exercises2
                }
            };

            //Items
            IList<Item> items = new List<Item>
            {
                new Item
                {
                    Name = "Item1",
                    Quantity = 2,
                    Price = 5.99
                },
                new Item
                {
                    Name = "Item2",
                    Quantity = 2,
                    Price = 5.99
                },
                new Item
                {
                    Name = "Item3",
                    Quantity = 2,
                    Price = 5.99
                }
            };
            
            gym = new Gym 
            { 
                Name = "Gym1",
                Items = items,
                Machines = machines
            };
            context.Gym.Add(gym);

            context.SaveChanges();
        }

        
        [Test]
        public async Task GetMachinesTestAsync() 
        {
            List<GymMachine> list = await inventoryInfo.GetMachines(gym.Id);
            List<GymMachine> emptylist = await inventoryInfo.GetMachines(5);

            Assert.That(list.Count, Is.Positive);
            Assert.That(emptylist.Count, Is.EqualTo(0));
            Assert.Pass();
        }
        [Test]
        public async Task GetMachineTestAsync()
        {
            var gm = await inventoryInfo.GetMachine(1);
            var gmNUll = await inventoryInfo.GetMachine(5);

            Assert.That(gm.MachineId, Is.EqualTo(1));
            Assert.That(gm != null, Is.EqualTo(true));
            Assert.That(gmNUll != null, Is.EqualTo(false));
            Assert.Pass();
        }
        [Test]
        public async Task AddMachineTestAsync()
        {
            var gymBefore = await inventoryInfo.GetMachines(gym.Id);

            inventoryInfo.AddMachine(gym, new GymMachine {
                Name = "Added Machine",
                Quantity = 2
            });

            var gymAfter = await inventoryInfo.GetMachines(gym.Id);

            Assert.That(gymAfter.Count, Is.EqualTo(gymBefore.Count + 1));
            Assert.Pass();
        }
        [Test]
        public async Task UpdateMachineTestAsync()
        {
            var machine = await inventoryInfo.GetMachine(1);
            var nameBefore = machine!.Name;

            var nameAfter = "New Name";
            machine.Name = nameAfter;
            inventoryInfo.UpdateMachine(gym, machine);

            var updated = await inventoryInfo.GetMachine(1);

            Assert.IsFalse(updated.Name == nameBefore);
            Assert.That(updated.Name, Is.EqualTo(nameAfter));
            Assert.Pass();
        }
        [Test]
        public async Task RemoveMachineTestAsync()
        {
            var gymBefore = await inventoryInfo.GetMachines(gym.Id);

            inventoryInfo.RemoveMachine(2);

            var gymAfter = await inventoryInfo.GetMachines(gym.Id);

            Assert.That(gymAfter.Count, Is.EqualTo(gymBefore.Count - 1));
            Assert.Pass();
        }


        [Test]
        public async Task GetExercisesTestAsync()
        {
            List<Exercise> list1 = await inventoryInfo.GetExercises(1);
            List<Exercise> list2 = await inventoryInfo.GetExercises(2);
            List<Exercise> emptylist = await inventoryInfo.GetExercises(5);

            Assert.That(list1.Count, Is.Positive);
            Assert.That(list2.Count, Is.EqualTo(1));
            Assert.That(emptylist.Count, Is.EqualTo(0));
            Assert.Pass();
        }
        [Test]
        public async Task GetExerciseTestAsync()
        {
            var ex = await inventoryInfo.GetExercise(1);
            var exNUll = await inventoryInfo.GetExercise(5);

            Assert.That(ex != null, Is.EqualTo(true));
            Assert.That(exNUll != null, Is.EqualTo(false));
            Assert.Pass();
        }
        [Test]
        public async Task AddExerciseTestAsync()
        {
            var machineBefore = await inventoryInfo.GetExercises(1);

            var machine = await inventoryInfo.GetMachine(1);

            await inventoryInfo.AddExercise(machine, new Exercise
            {
                Name = "Added Exercise"
            });

            var machineAfter = await inventoryInfo.GetExercises(1);

            Assert.That(machineAfter.Count, Is.EqualTo(machineBefore.Count + 1));
            Assert.Pass();
        }
        [Test]
        public async Task UpdateExerciseTestAsync()
        {
            var machine = await inventoryInfo.GetMachine(1);

            var exercise = await inventoryInfo.GetExercise(1);
            var nameBefore = exercise!.Name;

            var nameAfter = "New Name";
            exercise.Name = nameAfter;
            await inventoryInfo.UpdateExercise(machine, exercise);

            var updated = await inventoryInfo.GetExercise(1);

            Assert.IsFalse(updated.Name == nameBefore);
            Assert.That(updated.Name, Is.EqualTo(nameAfter));
            Assert.Pass();
        }
        [Test]
        public async Task RemoveExerciseTestAsync()
        {
            var machineBefore = await inventoryInfo.GetExercises(1);

            await inventoryInfo.RemoveExercise(2);

            var machineAfter = await inventoryInfo.GetExercises(1);

            Assert.That(machineAfter.Count, Is.EqualTo(machineBefore.Count - 1));
            Assert.Pass();
        }


        [Test]
        public async Task GetItemsTestAsync()
        {
            List<Item> list = await inventoryInfo.GetItems(gym.Id);
            List<Item> emptylist = await inventoryInfo.GetItems(5);

            Assert.That(list.Count, Is.Positive);
            Assert.That(emptylist.Count, Is.EqualTo(0));
            Assert.Pass();
        }
        [Test]
        public async Task GetItemTestAsync()
        {
            var i = await inventoryInfo.GetItem(1);
            var iNUll = await inventoryInfo.GetItem(5);

            Assert.That(i != null, Is.EqualTo(true));
            Assert.That(iNUll != null, Is.EqualTo(false));
            Assert.Pass();
            Assert.Pass();
        }
        [Test]
        public async Task AddItemTestAsync()
        {
            var gymBefore = await inventoryInfo.GetItems(gym.Id);

            await inventoryInfo.AddItem(gym, new Item
            {
                Name = "Added Item",
                Quantity = 2
            });

            var gymAfter = await inventoryInfo.GetItems(gym.Id);

            Assert.That(gymAfter.Count, Is.EqualTo(gymBefore.Count + 1));
            Assert.Pass();
        }
        [Test]
        public async Task UpdateItemTestAsync()
        {
            var item = await inventoryInfo.GetItem(1);
            var nameBefore = item!.Name;

            var nameAfter = "New Name";
            item.Name = nameAfter;
            await inventoryInfo.UpdateItem(gym, item);

            var updated = await inventoryInfo.GetItem(1);

            Assert.IsFalse(updated.Name == nameBefore);
            Assert.That(updated.Name, Is.EqualTo(nameAfter));
            Assert.Pass();
        }
        [Test]
        public async Task RemoveItemTestAsync()
        {
            var gymBefore = await inventoryInfo.GetItems(gym.Id);

            await inventoryInfo.RemoveItem(2);

            var gymAfter = await inventoryInfo.GetItems(gym.Id);

            Assert.That(gymAfter.Count, Is.EqualTo(gymBefore.Count - 1));
            Assert.Pass();
        }
    }
}
