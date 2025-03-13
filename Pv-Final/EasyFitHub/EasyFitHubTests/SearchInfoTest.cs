using EasyFitHub.Data;
using EasyFitHub.Models.Account;
using EasyFitHub.Models.Gym;
using EasyFitHub.Models.Profile;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFitHubTests
{
    internal class SearchInfoTest
    {
        Client account;

        private SearchInfo searchInfo;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            DbContextOptions<EasyFitHubContext> options = new DbContextOptionsBuilder<EasyFitHubContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            TestContext context = new TestContext(options);
            searchInfo = new SearchInfo(context);

            Gym g1 = new Gym { Name = "Gym1 with namex", Location = "Location1", IsConfirmed = true };
            Gym g2 = new Gym { Name = "GymABC", Location = "esta na LocationXYZ", IsConfirmed = true };
            context.Gym.AddRange(new List<Gym>
            {
                g1,
                new Gym { Name = "GymNamex", Location = "Location2",IsConfirmed = true },
                g2,
                new Gym { Name = "GymDEF", Location = "LocationXYZ",IsConfirmed = true }
            });

            account = new Client
            {
                Description = "Descrição do Cliente",
                Gender = Gender.MASCULINE,
                User = new User
                {
                    Email = "siTest@test.com",
                    UserName = "siTest",
                    BirthDate = DateOnly.Parse("1/1/2003"),
                    Password = "Teste",
                    Name = "name",
                    Surname = "Surname",
                },
                Biometrics = new Biometrics(),
                Data = new ClientData(),
            };
            context.Client.Add(
                account
            );

            context.GymClients.Add(
                new GymClient
                {
                    Client = account,
                    Gym = g1
                }
            );
            context.GymEmployees.Add(
                new GymEmployee
                {
                    Client = account,
                    Gym = g2
                }
            );

            context.SaveChanges();
        }

        [Test]
        public async Task TestGetGymsAsync()
        {
            IList<Gym> list = await searchInfo.GetGyms();

            Assert.That(list.Count, Is.Positive);
            Assert.Pass();
        }
        [Test]
        public async Task TestGetGymsByNameAsync()
        {
            IList<Gym> list = await searchInfo.GetGymsByName("namex");
            IList<Gym> list2 = await searchInfo.GetGymsByName("namenotexistent");

            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list2.Count, Is.EqualTo(0));
            Assert.Pass();
        }
        [Test]
        public async Task TestGetGymsByLocationAsync()
        {
            IList<Gym> list = await searchInfo.GetGymsByLocation("locationx");
            IList<Gym> list2 = await searchInfo.GetGymsByLocation("locationnotexistent");

            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list2.Count, Is.EqualTo(0));
            Assert.Pass();
        }
        [Test]
        public async Task TestGetGymsBySubscriptionAsync()
        {
            IList<Gym> list = await searchInfo.GetGymsBySubscription(account.User);

            Assert.That(list.Count, Is.EqualTo(2));
            Assert.Pass();
        }
    }
}
