using EasyFitHub.Data;
using EasyFitHub.Models.Account;
using EasyFitHub.Models.Payment;
using EasyFitHub.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFitHubTests
{
    internal class AuthenticationTest
    {
        private AuthenticationsInfo authenticationsInfo;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            DbContextOptions<EasyFitHubContext> options = new DbContextOptionsBuilder<EasyFitHubContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            TestContext context = new TestContext(options);
            authenticationsInfo = new AuthenticationsInfo(context);
        }
            
       
        [Test]
        public async Task CreateAccountTest()
        {
            User accountToCreate = new User 
            {
                BirthDate = new DateOnly(2003,1,1),
                Email = "test2@gmail.com",
                Name = "Test",
                UserName = "UserName2",
                Password = "password",
                Surname = "Test",
            };
            var user = await authenticationsInfo.CreateUser(accountToCreate);

            var queryAccount = await authenticationsInfo.GetAccount(user!.AccountId);

            Assert.That(accountToCreate.UserName, Is.EqualTo(user!.UserName));
            Assert.That(accountToCreate.UserName, Is.EqualTo(queryAccount!.UserName));

            Assert.Pass();
        }
        [Test]
        public async Task AuthenticationsTest()
        {
            Account accountToCreate = new User
            {
                BirthDate = new DateOnly(2003, 1, 1),
                Email = "test1@gmail.com",
                Name = "Test",
                UserName = "UserName",
                Password = "password",
                Surname = "Test",
            };
            var account = await authenticationsInfo.RegisterAccount(accountToCreate).FirstOrDefaultAsync();
            
            var loggedAccount = await authenticationsInfo.LoginAccount("password", account.UserName)!.FirstOrDefaultAsync();
            
            Assert.That(account!.UserName, Is.EqualTo(loggedAccount!.UserName));

            authenticationsInfo.UpdateAccountToken(account, "TokenTest");
            var updatedAccount = await authenticationsInfo.GetAccountWithToken("TokenTest");
            var res = await authenticationsInfo.IsLoged("TokenTest");

            Assert.That(updatedAccount!.UserName, Is.EqualTo(account.UserName));
            Assert.That(res, Is.True);

            Assert.Pass();
        }
    }
}
