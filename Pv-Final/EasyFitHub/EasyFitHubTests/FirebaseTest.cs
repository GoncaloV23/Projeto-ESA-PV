using EasyFitHub.Models.Miscalenous;
using EasyFitHub.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFitHubTests
{
    internal class FirebaseTest
    {
        private FirebaseService firebaseService;
        private HubImage img;
        private string temporaryImagePath;
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

            firebaseService = new FirebaseService(configuration);

            img = new HubImage
            {
                Name = "test",
                Description = "Desc"
            };
            temporaryImagePath = Path.GetTempFileName(); // Caminho para o arquivo temporário
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(temporaryImagePath);
        }

        [Test]
        public async Task CreateHubImageTest()
        {
            var data = "Example data for testing";
            File.WriteAllText(temporaryImagePath, data);

            HubImage result;
            using (Stream fileStream = File.OpenRead(temporaryImagePath))
            {
                result = await firebaseService.CreateHubImage(img, fileStream);
                Assert.That(result.Path, Is.Not.Null);
            }

            Assert.That(result.Path, Is.Not.Null);
        }

        [Test]
        public async Task DeleteHubImageTest()
        {
            var result = await firebaseService.DeleteHubImage(img);

            Assert.True(result);
        }
    }
}
