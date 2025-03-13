using EasyFitHub.Data;
using EasyFitHub.Models.Account;
using EasyFitHub.Models.Profile;
using Microsoft.EntityFrameworkCore;

namespace EasyFitHubTests
{
    internal class ProfileInfoTest
    {
        private ProfilesInfo profilesInfo;
        IList<Client> list;
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            DbContextOptions<EasyFitHubContext> options = new DbContextOptionsBuilder<EasyFitHubContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            TestContext context = new TestContext(options);
            profilesInfo = new ProfilesInfo(context);

            list = new List<Client>
            {
                new Client
                {
                    Biometrics = new Biometrics { },
                    Data = new ClientData
                    {
                        Location = "Location 1"
                    },
                    Gender = Gender.MASCULINE,
                    Description = "Description of client 1",
                    User = new User
                    {
                        UserName = "test1",
                        Email = "test1",
                        Password = "test1",
                        Name = "test1",
                        Surname = "test1",
                        BirthDate = new DateOnly(2002,2,27)
                    },
                },
                new Client
                {
                    Biometrics = new Biometrics { },
                    Data = new ClientData
                    {
                        Location = "Location 2"
                    },
                    Gender = Gender.FEMININE,
                    Description = "Description of client 2",
                    User = new User
                    {
                        UserName = "test2",
                        Email = "test2",
                        Password = "test2",
                        Name = "test2",
                        Surname = "test2",
                        BirthDate = new DateOnly(2002,2,28)
                    },
                },
                new Client
                {
                    Biometrics = new Biometrics { },
                    Data = new ClientData
                    {
                        Location = "Location 3"
                    },
                    Gender = Gender.MASCULINE,
                    Description = "Description of client 3",
                    User = new User
                    {
                        UserName = "test3",
                        Email = "test3",
                        Password = "test3",
                        Name = "test3",
                        Surname = "test3",
                        BirthDate = new DateOnly(2002,3,1)
                    },
                },
                new Client
                {
                    Biometrics = new Biometrics { },
                    Data = new ClientData
                    {
                        Location = "Location 4"
                    },
                    Gender = Gender.MASCULINE,
                    Description = "Description of client 4",
                    User = new User
                    {
                        UserName = "test4",
                        Email = "test4",
                        Password = "test4",
                        Name = "test4",
                        Surname = "test4",
                        BirthDate = new DateOnly(2002,2,27)
                    },
                },
                new Client
                {
                    Biometrics = new Biometrics { },
                    Data = new ClientData
                    {
                        Location = "Location 5"
                    },
                    Gender = Gender.FEMININE,
                    Description = "Description of client 5",
                    User = new User
                    {
                        UserName = "test5",
                        Email = "test5",
                        Password = "test5",
                        Name = "test5",
                        Surname = "test5",
                        BirthDate = new DateOnly(2002,2,28)
                    },
                },
                new Client
                {
                    Biometrics = new Biometrics { },
                    Data = new ClientData
                    {
                        Location = "Location 6"
                    },
                    Gender = Gender.MASCULINE,
                    Description = "Description of client 6",
                    User = new User
                    {
                        UserName = "test6",
                        Email = "test6",
                        Password = "test6",
                        Name = "test6",
                        Surname = "test6",
                        BirthDate = new DateOnly(2002,3,01)
                    },
                }
            };

            //Clients
            context.Client.AddRange(
                list
            );
            context.SaveChanges();
        }

        [Test]
        public async Task UpdateUserDataTest()
        {
            //Client
            var c1 = list[0];
            string newDescription = "New Description";
            c1.Description = newDescription;
            c1.Gender = Gender.FEMININE;

            await profilesInfo.UpdateClient(c1.ClientId, c1);

            //ClientData
            var c2 = list[1];
            string newLocation = "New Location";
            c2.Data.Location = newLocation;

            await profilesInfo.UpdateClient(c2.ClientId, c2);

            //Biometric
            var c3 = list[2];
            double newHeigth = 100;
            c3.Biometrics.Height = newHeigth;

            await profilesInfo.UpdateClient(c3.ClientId, c3);


            var client = profilesInfo.GetUser(c1.UserId);
            Assert.That(client.Description, Is.EqualTo(newDescription));
            Assert.That(client.Gender, Is.EqualTo(Gender.FEMININE));

            client = profilesInfo.GetUser(c2.UserId);
            Assert.That(client.Data.Location, Is.EqualTo(newLocation));

            client = profilesInfo.GetUser(c3.UserId);
            Assert.That(client.Biometrics.Height, Is.EqualTo(newHeigth));

            Assert.Pass();
        }
        [Test]
        public void DeleteUserTest()
        {
            var count = profilesInfo.GetClients().Count;

            profilesInfo.DeleteUser(list[3].User);
            profilesInfo.DeleteUser(list[4].User);
            profilesInfo.DeleteUser(list[5].User);

            Assert.That(profilesInfo.GetClients().Count, Is.EqualTo(count - 3));

            Assert.Pass();
        }
    }
}
