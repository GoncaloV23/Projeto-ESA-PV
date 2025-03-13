using EasyFitHub.Models.Payment;
using EasyFitHub.Services;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFitHubTests
{
    internal class StripeTest
    {
        StripeService stripeService;
        PaymentService paymentService;

        const string defaultCustomerID = "cus_PlbprNroroGeFr";
        const string defaultPaymentMethodID = "pm_1Ow57fRseCm2355tph3WmrRE";
        const string defaultAccountID = "acct_1Ow4beRpgJmjBFH7";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            StripeConfiguration.ApiKey = "sk_test_51OvLPTRseCm2355tSrhZeKuxcmiOVJT81xBnw6kKzx3yWExTrMRWOpQdv7WaLWWUpnCOVtcr9IoaDUyJFlN0e8rt00ojZxizlE";
            stripeService = new StripeService();
            paymentService = new PaymentService();
        }

        [Test]
        public async Task CustomerPaymentMethodTest()
        {
            var cusId = await stripeService.CreateCustomer(
                 "Name1",
                 "Test1",
                 "Name1Test1@gmail.com",
                 "Test Local1 Test"
            );

            var customer = await stripeService.GetCustomer(cusId);

            await stripeService.UpdateCustomer(
                cusId,
                "Name2",
                 "Test2",
                 "Name2Test2@gmail.com",
                 "Test Local2 Test"
            );
            var updatedCus = await stripeService.GetCustomer(cusId);

            await stripeService.DeleteCustomer(cusId);

            var removedCus = await stripeService.GetCustomer(cusId);

            Assert.That(customer.Name, Is.EqualTo("Name1 Test1"));
            Assert.That(updatedCus.Name, Is.EqualTo("Name2 Test2"));
            Assert.That(removedCus.Deleted, Is.True);


            var paymentMethods = await stripeService.GetPaymentMethods(defaultCustomerID);
            var sessionId = await stripeService.CreatePaymentMethodSession(defaultCustomerID, "https://no/url", "https://no/url/either");

            Assert.That(paymentMethods[0], Is.EqualTo(defaultPaymentMethodID));
            Assert.That(sessionId, Is.Not.Null);

            Assert.Pass();
        }
        [Test]
        public async Task GymBankAccountTest()
        {
            var accId = await stripeService.CreateAccount("GymTest", "GymTest@gmail.com", "This a GymTest Test!");
            var account = await stripeService.GetAccount(accId);

            await stripeService.DeleteAccount(accId);
            Stripe.Account? deletedAccount;

            try
            {
                deletedAccount = await stripeService.GetAccount(accId);
            }
            catch (Exception ex)
            {
                deletedAccount = null;
            }
            Assert.That(account.Id, Is.EqualTo(accId));
            Assert.That(deletedAccount, Is.Null);

            var createAccLinkUrl = await stripeService.CreateAccountLink(defaultAccountID, "https://no/url", "https://no/url/either");

            Assert.That(createAccLinkUrl, Is.Not.Null);

            Assert.Pass();
        }
        [Test]
        public async Task PlanTest()
        {
            var planId = await stripeService.CreatePlan("TestPlan", 10);
            var plan = await stripeService.GetPlan(planId);

            await stripeService.DeletePlan(planId);
            Stripe.Plan? deletedPlan;
            try
            {
                deletedPlan = await stripeService.GetPlan(planId);
            }
            catch (Exception ex)
            {
                deletedPlan = null;
            }

            Assert.That(plan.Id, Is.EqualTo(planId));
            Assert.That(deletedPlan, Is.Null);

            Assert.Pass();
        }
        [Test]
        public async Task SubscriptionTest()
        {
            var details = new PaymentDetails
            {
                PaymentDetailsId = 1,
                Status = Status.SUBSCRIPTION,
                Description = "Subscrição teste!",
                PaymentDate = new DateOnly(2024, 4, 1),
                Buyable = new EasyFitHub.Models.Payment.Subscription
                {
                    ClientDebitCard = new DebitCard
                    {
                        StripeCustomerId = "cus_PlbprNroroGeFr",
                        StripePaymentMethodId = "pm_1Ow57fRseCm2355tph3WmrRE"
                    },
                    GymBank = new EasyFitHub.Models.Payment.BankAccount
                    {
                        GymSubscriptionPrice = 5,
                        StripePlanId = await stripeService.CreatePlan
                        (
                            "Subscrição teste1",
                            5
                        ),
                        StripeBankId = "acct_1Ow4beRpgJmjBFH7",
                    },
                }
            };
            var subId = await paymentService.EnrollSubscription(details);
            var sub = await stripeService.GetSubscription(subId);
            var canceled = await paymentService.CancelSubscription(subId);



            Assert.That(subId, Is.Not.Null);
            Assert.That(sub, Is.Not.Null);
            Assert.That(canceled, Is.True);

            Assert.Pass();
        }
        [Test]
        public async Task PaymentTest()
        {
            var details = new PaymentDetails
            {
                PaymentDetailsId = 1,
                Status = Status.PAYED,
                Description = "Compra teste!",
                PaymentDate = new DateOnly(2024, 4, 1),
                Buyable = new Cart
                {
                    ClientDebitCard = new DebitCard
                    {
                        StripeCustomerId = "cus_PlbprNroroGeFr",
                        StripePaymentMethodId = "pm_1Ow57fRseCm2355tph3WmrRE"
                    },
                    GymBank = new EasyFitHub.Models.Payment.BankAccount
                    {
                        StripeBankId = "acct_1Ow4beRpgJmjBFH7",
                    },
                    Items = new List<CartItem>
                    {
                        new CartItem{ Item =
                        new EasyFitHub.Models.Inventory.Item
                        {
                            Price = 1
                        } },
                        new CartItem{ Item =
                        new EasyFitHub.Models.Inventory.Item
                        {
                            Price = 2
                        } },
                        new CartItem{ Item =
                        new EasyFitHub.Models.Inventory.Item
                        {
                            Price = 1
                        } },
                        new CartItem{ Item =
                        new EasyFitHub.Models.Inventory.Item
                        {
                            Price = 1
                        } },
                    }
                }
            };
            var res = await paymentService.MakePayment(details);

            Assert.That(res, Is.True);

            Assert.Pass();
        }
    }
}
