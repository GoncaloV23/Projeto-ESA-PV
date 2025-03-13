using EasyFitHub.Data;
using EasyFitHub.Models.Gym;
using EasyFitHub.Models.Profile;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFitHubTests
{
    
    internal class GymTest
    {
        private GymsInfo gymsInfo;
        TestContext context;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            DbContextOptions<EasyFitHubContext> options = new DbContextOptionsBuilder<EasyFitHubContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            context = new TestContext(options);
            gymsInfo = new GymsInfo(context);


            context.Gym.AddRange
            (
                new Gym 
                { 
                    Id = 11,
                    Name = "TESTTEST1", 
                    IsConfirmed = true 
                },
                new Gym
                {
                    Id = 12,
                    Name = "TESTTEST2",
                    IsConfirmed = false 
                },
                new Gym 
                { 
                    Id = 13,
                    Name = "TESTTEST3",
                    IsConfirmed = false 
                }
            );

            context.Client.AddRange
            (
                new Client
                {
                    ClientId = 110,
                    Biometrics = new Biometrics(),
                    Data = new ClientData(),
                    User = new EasyFitHub.Models.Account.User
                    {
                        BirthDate = new DateOnly(2003, 1, 1),
                        Email = "test10gmail.com",
                        Name = "Test",
                        UserName = "UserName10",
                        Password = "password",
                        Surname = "Test",
                    }
                },
                new Client
                {
                    ClientId = 111,
                    Biometrics = new Biometrics(),
                    Data = new ClientData(),
                    User = new EasyFitHub.Models.Account.User
                    {
                        BirthDate = new DateOnly(2003, 1, 1),
                        Email = "test11gmail.com",
                        Name = "Test",
                        UserName = "UserName11",
                        Password = "password",
                        Surname = "Test",

                    }
                },
                new Client
                {
                    ClientId = 112,
                    Biometrics = new Biometrics(),
                    Data = new ClientData(),
                    User = new EasyFitHub.Models.Account.User
                    {
                        BirthDate = new DateOnly(2003, 1, 1),
                        Email = "test12gmail.com",
                        Name = "Test",
                        UserName = "UserName12",
                        Password = "password",
                        Surname = "Test",
                    }
                }
            );

            context.SaveChanges();
        }


        [Test]
        public async Task ConfirmRemoveTest()
        {
            var gym2 = await gymsInfo.GetGym(12);
            var gym3 = await gymsInfo.GetGym(13);

            
            var countBefore = (await gymsInfo.GetGyms()).Count();

            var res1 = await gymsInfo.ConfirmGym(gym2!.Id);
            var res2 = await gymsInfo.DeleteGym(gym3!.Id);

            var countAfter = (await gymsInfo.GetGyms()).Count();

            var confirmedGym2 = await gymsInfo.GetGym(12);

            Console.WriteLine(countBefore);
            Console.WriteLine(countAfter);

            Assert.That(countBefore - countAfter, Is.EqualTo(1));
            Assert.That(res1, Is.True);
            Assert.That(res2, Is.True);
            Assert.That(confirmedGym2!.IsConfirmed, Is.True);
            
            Assert.Pass();
        }

        [Test]
        public async Task RequestsTest()
        {
            Client client1 = await context.Client.SingleOrDefaultAsync(c => c.ClientId == 110);

            var gym1 = await gymsInfo.GetGym(11);


            var beforeGym1 = (await gymsInfo.GetGym(11)).Requests.Count;

            await gymsInfo.AddRequest(gym1!, client1);

            var afterAddGym1 = (await gymsInfo.GetGym(11)).Requests.Count;

            await gymsInfo.RemoveRequest(gym1!, client1);

            var afterRemoveGym1 = (await gymsInfo.GetGym(11)).Requests.Count;


            Assert.That(beforeGym1 - afterAddGym1, Is.EqualTo(-1));
            Assert.That(afterRemoveGym1 - afterAddGym1, Is.EqualTo(-1));
            Assert.Pass();
        }
        [Test]
        public async Task EmployeesTest()
        {
            var gym1 = await gymsInfo.GetGym(11);
            Client client2 = await context.Client.SingleOrDefaultAsync(c => c.ClientId == 111);

            var beforeGym1 = (await gymsInfo.GetGym(11)).GymEmployees.Count;

            await gymsInfo.AddEmployee(client2, gym1!, Role.PT);

            var afterAddGym1 = (await gymsInfo.GetGym(11)).GymEmployees.Count;

            await gymsInfo.RemoveEmployee(client2, gym1!);

            var afterRemoveGym1 = (await gymsInfo.GetGym(11)).GymEmployees.Count;
            
            Assert.That(beforeGym1 - afterAddGym1, Is.EqualTo(-1));
            Assert.That(afterRemoveGym1 - afterAddGym1, Is.EqualTo(-1)); 
            Assert.Pass();
        }
            [Test]
        public async Task ClientsTest()
        {
            var gym1 = await gymsInfo.GetGym(11);

            Client client3 = await context.Client.SingleOrDefaultAsync(c => c.ClientId == 112);

            var beforeGym1 = (await gymsInfo.GetGym(11)).GymClients.Count;
            

            await gymsInfo.AddClient(client3, gym1!); 
            //await gymsInfo.AddEmployeeClientRelation(client2, client3, beforeGym1!); Console.WriteLine("Aqui5");

            var afterAddGym1 = (await gymsInfo.GetGym(11)).GymClients.Count;
            

            await gymsInfo.RemoveClient(client3, gym1!);
            //await gymsInfo.RemoveEmployeeClientRelation(client2, client3, afterAddGym1!);

            var afterRemoveGym1 = (await gymsInfo.GetGym(11)).GymClients.Count;

            Assert.That(beforeGym1 - afterAddGym1, Is.EqualTo(-1));
            

            Assert.That(afterRemoveGym1 - afterAddGym1, Is.EqualTo(-1));
            
            Assert.Pass();
        }
        [Test]
        public async Task RelationshipsTest()
        {
            var gym1 = await gymsInfo.GetGym(11);

            Client client1 = await context.Client.SingleOrDefaultAsync(c => c.ClientId == 110);
            Client client2 = await context.Client.SingleOrDefaultAsync(c => c.ClientId == 111);

            var aux = (await gymsInfo.GetGym(11)).GymEmployees.FirstOrDefault();
            var beforeGym1 = (aux == null)? 0: aux.GymClients.Count;


            await gymsInfo.AddClient(client1, gym1!);
            await gymsInfo.AddEmployee(client2, gym1!, Role.PT);
            
            
            await gymsInfo.AddEmployeeClientRelation(client2, client1, gym1!);

            var afterAddGym1 = (await gymsInfo.GetGym(11)).GymEmployees.FirstOrDefault().GymClients.Count;


            await gymsInfo.RemoveEmployeeClientRelation(client2, client1, gym1!);

            var afterRemoveGym1 = (await gymsInfo.GetGym(11)).GymEmployees.FirstOrDefault().GymClients.Count;

            Assert.That(beforeGym1 - afterAddGym1, Is.EqualTo(-1));


            Assert.That(afterRemoveGym1 - afterAddGym1, Is.EqualTo(-1));

            Assert.Pass();
        }
    }
}
