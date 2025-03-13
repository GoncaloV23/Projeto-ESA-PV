using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyFitHub.Data;
using EasyFitHub.Models.Payment;
using EasyFitHub.Services;
using Stripe.Checkout;
using Stripe;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System.Collections;
using EasyFitHub.Models.Gym;
using EasyFitHub.Models.Profile;
using EasyFitHub.Models.Inventory;
using EasyFitHub.Models.Account;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Stripe.FinancialConnections;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Elfie.Serialization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Mono.TextTemplating;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.CodeDom.Compiler;
using System.Reflection.PortableExecutable;
using System.Threading;
using EasyFitHub.Utils;
using System.Security.Principal;

namespace EasyFitHub.Controllers
{
    public class PaymentsController : Controller
    {
        /// <summary>
        /// Author: André P.
        /// Este controlador utiliza um PaymentInfo, PaymentService e StripeService para gerir as operaçoes de Pagamento
        /// O controlador tambem utiliza o Authorization e Authentication Services que auxiliam na gestão das permissões exigidas por cada método
        /// As views relacionadas a este Controlador são:
        ///     GymSubscription, PaymentMethod, SubscriptionPlan
        /// </summary>
        private readonly ILogger<PaymentsController> _logger;
        private readonly StripeService _stripeService;
        private readonly PaymentService _paymentService;
        private readonly PaymentsInfo _paymentInfo;
        private readonly AuthenticationService _authenticationService;
        private readonly AuthorizationService _authorizationService;
        /// <summary>
        /// constructor for the controller
        /// </summary>
        /// <param name="logger">used for debuging</param>
        /// <param name="payService">used for Creating Payments</param>
        /// <param name="stripe">used for handling Payment Details</param>
        /// <param name="authService">used for security to check for authentication</param>
        /// <param name="authoService">used for security to check for authorization</param>
        /// <param name="context">used to instantiate a PaymentInfo which handled DataBase operations</param>
        public PaymentsController(ILogger<PaymentsController> logger, PaymentService payService, StripeService stripe, AuthenticationService authService, AuthorizationService authoService, EasyFitHubContext context)
        {
            _logger = logger;
            _stripeService = stripe;
            _paymentService = payService;
            _paymentInfo = new PaymentsInfo(context, logger);
            _authenticationService = authService;
            _authorizationService = authoService;

        }
        //Client PaymentMethod
        /// <summary>
        /// method used to create a payment method using Stripe 
        /// it also creates a DebitCard instance for the client in the Database
        /// The returning View contains the PaymentMethodSession ID in ViewData
        /// </summary>
        /// <param name="clientId">Id of a client in the database</param>
        /// <returns>View PaymentMethod</returns>
        [HttpGet]
        public async Task<ActionResult> PaymentMethodSession(int clientId)
        {            
            var sessionAccount = await _authenticationService.GetAccount("token", HttpContext.Session.GetString("AccessToken"));
            var client = await _paymentInfo.GetClient(clientId);
            if (sessionAccount == null || client == null || sessionAccount.AccountId != client.UserId) return RedirectToAction("NotAuthorized");

            var clientCard = await _paymentInfo.GetClientCard(clientId);
            if (clientCard == null || clientCard.StripeCustomerId == null)
            { 
                
                var customerId = await _stripeService.CreateCustomer
                (
                    client.User.Name,
                    client.User.Surname,
                    client.User.Email,
                    client.Data.Location
                );
               
                clientCard = new DebitCard {
                    Client = client,
                    StripeCustomerId = customerId,
                };

                await _paymentInfo.CreateDebitCard(clientCard);
            }

            var successUrl = Url.Action("PaymentMethodSessionSucess", "Payments", new { clientId = clientId }, Request.Scheme);
            var cancelUrl = Url.Action("PaymentMethodSessionFail", "Payments", new { clientId = clientId }, Request.Scheme);
            _logger.LogInformation("\n\n\n\nSucess:\n" + successUrl + "\n\n\n\n");
            _logger.LogInformation("\n\n\n\nFail:\n" + cancelUrl + "\n\n\n\n");

            var session = await _stripeService.CreatePaymentMethodSession
            (
                clientCard.StripeCustomerId,
                successUrl,
                cancelUrl
            );

            ViewData["SessionId"] = session;
            return View("PaymentMethod");
        }
        /// <summary>
        /// method used for routing the client to the success page when creating a Payment method 
        /// </summary>
        /// <param name="clientId">Id of the Client</param>
        /// <returns> redirects to Profile/Index which is the clients Profile page
        /// otherwise if the authenticated user has no permissions he is redirected to NotAuthorized Page</returns>
        public async Task<ActionResult> PaymentMethodSessionSucess(int clientId)
        {
            _logger.LogInformation("\n\n\n\nPayment Method Sucess!\n\n\n\n");
            var sessionAccount = await _authenticationService.GetAccount("token", HttpContext.Session.GetString("AccessToken"));
            var clientCard = await _paymentInfo.GetClientCard(clientId);

            if (sessionAccount == null || clientCard == null || sessionAccount.AccountId != clientCard.Client.UserId) return RedirectToAction("NotAuthorized");

            if (clientCard.StripeCustomerId == null) return RedirectToAction("PaymentMethodSessionFail", new { clientId = clientId, isError = true });
            
            var paymentIds = await _stripeService.GetPaymentMethods(clientCard.StripeCustomerId);
            if (paymentIds == null || paymentIds.Count == 0) return RedirectToAction("PaymentMethodSessionFail", new { clientId = clientId, isError = true });

            clientCard.StripePaymentMethodId = paymentIds[0];
            await _paymentInfo.UpdateDebitCard(clientCard);

            return RedirectToAction("Index", "Profile", new { userId = clientId });
        }
        /// <summary>
        /// method used for routing the client to the failure page when creating a Payment method 
        /// </summary>
        /// <param name="clientId">Id of the Client</param>
        /// <param name="isError">optional argument, not to be normaly used</param>
        /// <returns>redirects to the Client's Profile page at Profile/Index/</returns>
        public ActionResult PaymentMethodSessionFail(int clientId, bool isError = false)
        {
            _logger.LogInformation("\n\n\n\nPayment Method Fail!\n\n\n\n");
            if(isError)ViewData["ResultMessage"] = "Something Went Wrong/nThe Payment Method has not been changed!";
            return RedirectToAction("Index", "Profile", new { userId = clientId });
        }
        /// <summary>
        /// creates an account link for a Gym's Bank Account in Stripe
        /// </summary>
        /// <param name="gymId">Id of the Gym</param>
        /// <returns>redirects to either the success or failure page.
        /// otherwise if the authenticated user has no permissions he is redirected to NotAuthorized Page</returns>
        public async Task<ActionResult> CreateAccountLink(int gymId)
        {
            var sessionAccount = await _authenticationService.GetAccount("token", HttpContext.Session.GetString("AccessToken"));
            var gymBank = await _paymentInfo.GetGymBank(gymId);

            if (sessionAccount == null) return RedirectToAction("NotAuthorized");
            if (gymBank == null || string.IsNullOrEmpty(gymBank.StripeBankId)) return await CreateAccount(gymId);

            var isManager = _authorizationService.IsManager(sessionAccount, gymBank.Gym);
            if(!isManager) return RedirectToAction("NotAuthorized");

            var refreshUrl = Url.Action("AccountLinkSucessFail", "Payments", new { gymId = gymId }, Request.Scheme);
            var returnUrl = Url.Action("AccountLinkSucessSucess", "Payments", new { gymId = gymId }, Request.Scheme);

            var url = await _stripeService.CreateAccountLink(gymBank.StripeBankId, refreshUrl, returnUrl);
            if (string.IsNullOrEmpty(url)) return RedirectToAction("AccountLinkSucessFail", new { gymId = gymId , isError = true }); 

            return Redirect(url);
        }
        /// <summary>
        /// Used for creating a Bank Account for a gym in DB and in Stripe
        /// </summary>
        /// <param name="gymId">Id of the gym</param>
        /// <returns>Redirects to CreateAccountLink Page
        /// otherwise, if there is an error in creating a BankAccount then it redirects to NotAuthorized page</returns>
        [HttpPost]
        public async Task<ActionResult> CreateAccount(int gymId)
        {
            var res = await _authorizationService.GetGymAndAccount(gymId, HttpContext.Session);

            if(res.Gym == null || res.Account == null || !_authorizationService.IsManager(res.Account, res.Gym)) return RedirectToAction("NotAuthorized");

            var accountId = await _stripeService.CreateAccount
            (
                res.Gym.Name,
                res.Account.Email,
                res.Gym.Description
            );
            if (!string.IsNullOrEmpty(accountId))
            {
                var account = new Models.Payment.BankAccount
                {
                    Gym = res.Gym,
                    StripeBankId = accountId,
                    StripePlanId = null,
                    GymSubscriptionPrice = 0
                };
                await _paymentInfo.CreateBankAccount(account);
                return RedirectToAction("CreateAccountLink", new { gymId = gymId });
            }
            return RedirectToAction("NotAuthorized");
        }
        /// <summary>
        /// Redirects to AccountLink Success page for a Gym
        /// </summary>
        /// <param name="gymId">Id of the Gym</param>
        /// <returns>redirects to the Gym's index page</returns>
        public ActionResult AccountLinkSucessSucess(int gymId) 
        {
            _logger.LogInformation("\n\n\n\nAccount Link Sucess\n\n\n\n\n");
            return RedirectToAction("Index", "Gyms", new { gymId = gymId });
        }
        /// <summary>
        /// Redirects to AccountLink failure page for a Gym
        /// </summary>
        /// <param name="gymId">Id of the Gym</param>
        /// <param name="isError">used if it failure is ocured for an unexpectedError</param>
        /// <returns>redirects to the Gym's index page</returns>
        public ActionResult AccountLinkSucessFail(int gymId, bool isError = false) 
        {
            _logger.LogInformation("\n\n\n\nAccount Link Fail\n\n\n\n\n");
            return RedirectToAction("Index", "Gyms", new { gymId = gymId });
        }

        //Payment
        /// <summary>
        /// Used when purchasing in a gym's store, It creates a PaymentDetail instance in the DB
        /// </summary>
        /// <param name="gymId">Id of a gym</param>
        /// <param name="items">list of items to be purchased off of a gym's store</param>
        /// <returns>redirects to the Payment Page,if there are no authorizations redirects to Home/Index
        /// , if there are errors in the items listing redirects to Inventory/Items</returns>
        [HttpPost]
        public async Task<ActionResult> ConfirmPayment(int gymId, List<int> items)
        {
            _logger.LogInformation($"\n\n\n\nConfirmPayment\n\n\n\n");
            var res = await _authorizationService.isAuthorized(gymId, HttpContext.Session);
            if (res.Account == null || res.Gym == null) return Json(new { redirectTo = Url.Action("Index", "Home") });

            

            if (items == null || items.Count == 0) return Json(new { redirectTo = Url.Action("Items", "Inventory", new { gymId = gymId }) });
            var gymItems = res.Gym.Items.Where(i => items.Any(id => id == i.ItemId)).ToList();
            if (gymItems.Count == 0) return Json(new { redirectTo = Url.Action("Items", "Inventory", new { gymId = gymId }) });
            
            

            var gymBank = await _paymentInfo.GetGymBank(gymId);
            var card = await _paymentInfo.GetClientCard(res.Account.AccountId);
            if (gymBank == null) return Json(new { redirectTo = Url.Action("Index", "Home") });
            if (card == null) return Json(new { redirectTo = Url.Action("PaymentMethodSession", new { clientId = res.Account.AccountId }) });
            
            
            var cartItems = new List<CartItem>();
            foreach(var item in gymItems)
            {
                cartItems.Add(new CartItem 
                { 
                    Item = item
                });
            }

            string descText = "[";

            foreach (Item item in gymItems)
                descText += item.Name + ", ";

            descText += "]";


            var details = new PaymentDetails
            {
                PaymentDate = DateOnly.FromDateTime(DateTime.Now),
                Buyable = new Cart
                {
                    Items = cartItems,
                    GymBank = gymBank,
                    ClientDebitCard = card,
                },
                Description = descText,
                Status = Status.PENDENT
            };

            details = await _paymentInfo.AddPaymentDetails(details);

            return Json(new { redirectTo = Url.Action("PaymentPage", new { detailsId = details.PaymentDetailsId }) }); 
        }
        /// <summary>
        /// method used to route the user to a diferent page
        /// </summary>
        /// <param name="detailsId">Id of PaymentDetails, created when there is a pending Payment</param>
        /// <returns>redirects to Payments/GymSubscription view passing the PaymentDetails</returns>
        public async Task<ActionResult> PaymentPage(int detailsId)
        {
            var details = await _paymentInfo.GetPaymentDetails(detailsId);
            if(details == null) return NotFound();
            return View("GymSubscription", details); 
        }
        /// <summary>
        /// Used to pay using PaymentService
        /// it updates the PaymentDetails instance in the Database to declare a PAYED status
        /// </summary>
        /// <param name="detailsId">unpayed PaymentDetails Id</param>
        /// <returns>redirects to Gyms/Index if the Payment is sucessful, otherwise
        /// redirects to Home/Index</returns>
        [HttpPost]
        public async Task<ActionResult> Payment(int detailsId)
        {
            var details = await _paymentInfo.GetPaymentDetails(detailsId);
            if(details == null)return RedirectToAction("Index", "Home");

            var result = await _paymentService.MakePayment(details);

            if (result) {
                details.Status = Status.PAYED;
                await _paymentInfo.UpdatePaymentDetail(detailsId, details);

                var res = await _authorizationService.GetGymAndAccount(-1, HttpContext.Session);

                EmailSender.SendReceipt(res.Account, _paymentService.MakeReceipt(details));
                return RedirectToAction("Index", "Gyms",  new {gymId = details.Buyable.GetSeller().GymId});
            }
            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        /// used to view a subsription plan's details
        /// </summary>
        /// <param name="gymId">Id of the Gym with the subscription plan</param>
        /// <returns>redirects to the corresponing View passing a GymBank instance,
        /// otherwise if there are no authorizations of authentication it redirects to NotAuthorized Page</returns>
        public async Task<ActionResult> SubscriptionPlan(int gymId)
        {
            var res = await _authorizationService.GetGymAndAccount(gymId, HttpContext.Session);

            if (res.Gym == null || res.Account == null || !_authorizationService.IsManager(res.Account, res.Gym)) return RedirectToAction("NotAuthorized");

            var gymBank = await _paymentInfo.GetGymBank(gymId);
            if(gymBank == null || string.IsNullOrEmpty(gymBank.StripeBankId)) return await CreateAccount(gymId);

            var stripeAccount = await _stripeService.GetAccount(gymBank.StripeBankId);
            if(!stripeAccount.PayoutsEnabled) return RedirectToAction("CreateAccountLink", new { gymId = gymId });

            return View(gymBank);
        }
        /// <summary>
        /// redirects to a page used to confirms a Gym's Subscription for a client
        /// it aquires the Client instance via Authentication Session
        /// </summary>
        /// <param name="gymId">Id of the Gym</param>
        /// <returns>redirects to GymSubscriptionView with PaymentDetails instance, otherwise
        /// redirects to ConfirmUnsubscription if it is a gymClient or NotAuthorized Page if there is no authorization</returns>
        [HttpGet]
        public async Task<ActionResult> ConfirmSubscription(int gymId)
        {
            var authorization = await _authorizationService.isAuthorized(gymId, HttpContext.Session);

            if (authorization.Account == null || authorization.Gym == null) return RedirectToAction("NotAuthorized");
            if (authorization.IsAuthorizedToRead) return RedirectToAction("ConfirmUnsubscription", new { gymId = gymId });
            
            var client = await _paymentInfo.GetClient(authorization.Account.AccountId);
            var clientCard = await _paymentInfo.GetClientCard(client.UserId);
            var gymBank = await _paymentInfo.GetGymBank(authorization.Gym.Id);

            if (client == null || clientCard == null || string.IsNullOrEmpty(clientCard.StripePaymentMethodId)) 
                return RedirectToAction("PaymentMethodSession", new { clientId = authorization.Account.AccountId });

            var details = new PaymentDetails
            {
                Status = Status.SUBSCRIPTION,
                Description = "subscription_" + authorization.Gym.Name + "_" + clientCard.StripeCustomerId,
                PaymentDate = DateOnly.FromDateTime(DateTime.Now),
                Buyable = new Models.Payment.Subscription
                {
                    ClientDebitCard = clientCard,
                    GymBank = gymBank
                }
            };
            return View("GymSubscription", details);
        }
        /// <summary>
        /// used by platform clients to request a subcription for a gym
        /// </summary>
        /// <param name="clientId">Id of the Client</param>
        /// <param name="gymId">Id of the Gym</param>
        /// <returns>redirects to Gyms/EnlistGym Method</returns>
        [HttpPost]
        public async Task<ActionResult> RequestSubscription(int clientId, int gymId)
        {
            var clientCard = await _paymentInfo.GetClientCard(clientId);
            var gymBank = await _paymentInfo.GetGymBank(gymId);

            if (_authorizationService.IsClient(clientCard.Client.User, gymBank.Gym)) return RedirectToAction("Index", "Gyms", new { gymId = gymId });

            var details = await _paymentInfo.GetSubscriptionPaymentDetail(gymId, clientId);
            if (details == null)
            {
                details = new PaymentDetails
                {
                    Status = Status.PENDENT,
                    Description = "subscription_" + gymBank.Gym.Name + "_" + clientCard.StripeCustomerId,
                    PaymentDate = DateOnly.FromDateTime(DateTime.Now),
                    Buyable = new Models.Payment.Subscription
                    {
                        ClientDebitCard = clientCard,
                        GymBank = gymBank
                    }
                };
                await _paymentInfo.CreatePaymentDetail(details);
            }
            else
            {
                if(details.Status != Status.CANCELED) 
                    return RedirectToAction("Index", "Gyms", new { gymId = gymId });

                details.Status = Status.PENDENT;

                await _paymentInfo.UpdatePaymentDetailWithSubscription(details.Buyable.BuyableId, details);
            }

            return RedirectToAction("EnlistGym", "Gyms", new { gymId = gymId });
            
        }
        /// <summary>
        /// confirms and creates a Subscritpion for a client in the PaymentService 
        /// </summary>
        /// <param name="clientId">Id of the clientr</param>
        /// <param name="gymId">Id of the Gym</param>
        /// <param name="role">client's role in the gym</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Subscribe(int clientId, int gymId, Role role)
        {
            var res = await _authorizationService.isAuthorized(gymId, HttpContext.Session);
            if(!res.IsAuthorizedToEdit) return RedirectToAction("Index", "Gyms", new { gymId = gymId });

            var details = await _paymentInfo.GetSubscriptionPaymentDetailWithStatus(gymId, clientId, Status.PENDENT);
            if(details == null) return RedirectToAction("Index", "Gyms", new { gymId = gymId });

            if (role == Role.CLIENT)
            {
                details.Status = Status.SUBSCRIPTION;
                var sub = details.Buyable as Models.Payment.Subscription;
                sub.StripeSubscriptionId = await _paymentService.EnrollSubscription(details);

                var receipt = _paymentService.MakeReceipt(details);

                EmailSender.SendReceipt(res.Account, receipt);
            }
            else
            {
                details.Status = Status.EMPLOYEE;
            }

            await _paymentInfo.UpdatePaymentDetailWithSubscription(details.Buyable.BuyableId, details);

            return RedirectToAction("ConfirmRequest", "Gyms", new { gymId = gymId, accountId = clientId, role = role });
        }
        /// <summary>
        /// deny's a client's request for a gymSubscription
        /// </summary>
        /// <param name="clientId">Id of the client</param>
        /// <param name="gymId">Id of the gym</param>
        /// <returns>redirects to DenyRequest method in Gym's controller, otherwise if there are no authorizations or if there are errors,
        /// redirects to Gyms/Index view</returns>
        [HttpPost]
        public async Task<ActionResult> DenySubscription(int clientId, int gymId)
        {
            var res = await _authorizationService.isAuthorized(gymId, HttpContext.Session);
            if (!res.IsAuthorizedToEdit) return RedirectToAction("Index", "Gyms", new { gymId = gymId });

            var details = await _paymentInfo.GetSubscriptionPaymentDetailWithStatus(gymId, clientId, Status.PENDENT);
            if (details == null) return RedirectToAction("Index", "Gyms", new { gymId = gymId });

            
            details.Status = Status.CANCELED;

            await _paymentInfo.UpdatePaymentDetailWithSubscription(details.Buyable.BuyableId, details);

            return RedirectToAction("DenyRequest", "Gyms", new { gymId = gymId, accountId = clientId});
        }
        /// <summary>
        /// passes Subscription status and details of a client to the corresponing view
        /// </summary>
        /// <param name="gymId">Id of the Gym</param>
        /// <returns>redirects to ConfirmSubscription Method if there is no subscription found,
        /// otherwise returns GymSubscription View with the details</returns>
        [HttpGet]
        public async Task<ActionResult> ConfirmUnsubscription(int gymId)
        {
            var authorization = await _authorizationService.isAuthorized(gymId, HttpContext.Session);
            if (authorization.IsAuthorizedToRead)
            {

                var details = await _paymentInfo.GetSubscriptionPaymentDetail(gymId, authorization.Account!.AccountId);
                _logger.LogInformation("\n\n\n\nDETAILS: " + details + "\n\n\n\n");

                if (details == null) return RedirectToAction("ConfirmSubscription", new {gymId = gymId});

                ViewData["IsSubscribed"] = true;
                return View("GymSubscription", details);
            }
            return RedirectToAction("NotAuthorized");
        }
        /// <summary>
        /// Handles the unsubscription process for a client from a gym.
        /// </summary>
        /// <param name="clientId">The ID of the client.</param>
        /// <param name="gymId">The ID of the gym.</param>
        /// <returns>
        /// Redirects to RemoveEmployee action if the client is an employee.
        /// Otherwise, redirects to RemoveClient action.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> Unsubscribe(int clientId, int gymId)
        {
            var res = await _authorizationService.isAuthorized(gymId, HttpContext.Session);
            if(!res.IsAuthorizedToEdit && (res.Account == null || res.Account.AccountId != clientId)) return RedirectToAction("NotAuthorized");

            var details = await _paymentInfo.GetSubscriptionPaymentDetail(gymId, clientId);
            if(details == null) return RedirectToAction("Index", "Home");
            var status = details.Status;
            

            var sub = details.Buyable as Models.Payment.Subscription;
            if (!string.IsNullOrEmpty(sub.StripeSubscriptionId))
                await _paymentService.CancelSubscription(sub.StripeSubscriptionId);

            details.Status = Status.CANCELED;
            sub.StripeSubscriptionId = null;
            await _paymentInfo.UpdatePaymentDetailWithSubscription(details.BuyableId, details);

            if (status == Status.EMPLOYEE)
                return RedirectToAction("RemoveEmployee", "Gyms", new { gymId = gymId, accountId = clientId });
            else //if (status == Status.SUBSCRIPTION)
                return RedirectToAction("RemoveClient", "Gyms", new { gymId = gymId, accountId = clientId });
        }
        /// <summary>
        /// Handles the unsubscription process for a client from a gym.
        /// </summary>
        /// <param name="clientId">The ID of the client.</param>
        /// <param name="gymId">The ID of the gym.</param>
        /// <returns>
        /// Redirects to RemoveEmployee action if the client is an employee.
        /// Otherwise, redirects to RemoveClient action.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> UnsubscribeAll(int gymId)
        {
            var res = await _authorizationService.isAuthorized(gymId, HttpContext.Session);
            if (!res.IsAuthorizedToEdit && (res.Account == null || res.Account.AccountType == Models.Account.AccountType.USER)) return RedirectToAction("NotAuthorized");

            var clients = res.Gym.GymClients;
            foreach(var client in clients){
                var details = await _paymentInfo.GetSubscriptionPaymentDetail(gymId, client.ClientId);
                if (details == null) continue;
                var status = details.Status;


                var sub = details.Buyable as Models.Payment.Subscription;
                if (!string.IsNullOrEmpty(sub.StripeSubscriptionId))
                    await _paymentService.CancelSubscription(sub.StripeSubscriptionId);

                details.Status = Status.CANCELED;
                sub.StripeSubscriptionId = null;
                await _paymentInfo.UpdatePaymentDetailWithSubscription(details.BuyableId, details);
            }
            
            return RedirectToAction("DeleteGym", "Gyms", new { gymId = gymId, isRedirected = true});
        }

        /// <summary>
        /// Removes the account associated with a gym, including its bank information and plan.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <returns>
        /// Redirects to the home page if the account removal is successful.
        /// Otherwise, redirects to the NotAuthorized action.
        /// </returns>

        [HttpPost]
        public async Task<ActionResult> RemoveAccount(int gymId)
        {
            var authorization = _authorizationService.isAuthorized(gymId, HttpContext.Session);
            if (authorization.Result.IsAuthorizedToEdit)
            {
                var gymBank = await _paymentInfo.GetGymBank(gymId);
                var result = _stripeService.DeletePlan(gymBank.StripePlanId);
                if (result.IsCompletedSuccessfully)
                {
                    var result2 = _stripeService.DeleteAccount(gymBank.StripeBankId);
                    if (result2.IsCompletedSuccessfully)
                    {
                        _paymentInfo.DeleteBank(gymId);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return RedirectToAction("NotAuthorized", "Home");
        }
        /// <summary>
        /// Updates customer in Stripe.
        /// </summary>
        /// <param name="clientId">The ID of the client.</param>
        /// <returns>
        /// Redirects to the home page if the update is successful.
        /// Otherwise, redirects to the NotAuthorized action.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> UpdateCustomer(int clientId)
        {
            var authorization = _authenticationService.GetAccount("token", HttpContext.Session.GetString("AccessToken"));
            if (authorization.IsCompletedSuccessfully)
            {
                var acc = (User)authorization.Result;
                var client = await _paymentInfo.GetClient(acc.AccountId);
                var card = await _paymentInfo.GetClientCard(clientId);
                if (!string.IsNullOrEmpty(card.StripeCustomerId))
                {
                    var x = _stripeService.UpdateCustomer(
                        card.StripeCustomerId,
                        acc.Name, acc.Surname,
                        acc.Email,
                        client.Data.Location
                    );
                    if (x.IsCompletedSuccessfully) {
                        card.StripeCustomerId = x.Result;
                        _paymentInfo.UpdateDebitCard(card);
                        return Json(x.Result);
                    }
                }
                
            }
            return RedirectToAction("NotAuthorized", "Home");
        }
        /// <summary>
        /// Removes a customer's informationfrom Stripe and from DB
        /// </summary>
        /// <param name="clientId">The ID of the client.</param>
        /// <returns>
        /// Redirects to the home page if the removal is successful.
        /// Otherwise, redirects to the NotAuthorized action.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> RemoveCustomer(int clientId)
        {
            var authorization = _authenticationService.GetAccount("token", HttpContext.Session.GetString("AccessToken"));
            var card = await _paymentInfo.GetClientCard(clientId);
            if (authorization.IsCompletedSuccessfully && !string.IsNullOrEmpty(card.StripeCustomerId))
            {
                var removal = _stripeService.DeleteCustomer(card.StripeCustomerId);
                if (removal.IsCompletedSuccessfully)
                {
                    card.StripeCustomerId = null;
                    await _paymentInfo.UpdateDebitCard(card);
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("NotAuthorized", "Home");
        }
        /// <summary>
        /// Deletes a payment method associated with a customer, removing it's StripeMethodId from the card
        /// </summary>
        /// <param name="clientId">The ID of the client.</param>
        /// <returns>
        /// Redirects to the home page if the deletion is successful.
        /// Otherwise, redirects to the NotAuthorized action.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> DeletePaymentMethod(int clientId)
        {
            var authorization = _authenticationService.GetAccount("token", HttpContext.Session.GetString("AccessToken"));
            var card = await _paymentInfo.GetClientCard(clientId);
            if (authorization.IsCompletedSuccessfully && !string.IsNullOrEmpty(card.StripePaymentMethodId))
            {
                var removal = _stripeService.DeletePaymentMethod(card.StripePaymentMethodId);
                if (removal.IsCompletedSuccessfully)
                {
                    card.StripePaymentMethodId = null;
                    await _paymentInfo.UpdateDebitCard(card);
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("NotAuthorized", "Home");
        }
        /// <summary>
        /// Sets a default payment method for a customer.
        /// </summary>
        /// <param name="clientId">The ID of the client.</param>
        /// <param name="stripePaymentMethodId">The ID of the Stripe payment method.</param>
        /// <returns>
        /// Redirects to the home page if the update is successful.
        /// Otherwise, redirects to the NotAuthorized action.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> SetDefaultPaymentMethod(int clientId, string stripePaymentMethodId)
        {
            var authorization = _authenticationService.GetAccount("token", HttpContext.Session.GetString("AccessToken"));
            var card = await _paymentInfo.GetClientCard(clientId);
            if (authorization.IsCompletedSuccessfully && !string.IsNullOrEmpty(card.StripeCustomerId))
            {
                var update = _stripeService.SetDefaultPaymentMethods(card.StripeCustomerId, stripePaymentMethodId);
                if (update.IsCompletedSuccessfully)
                {
                    card.StripePaymentMethodId = stripePaymentMethodId;
                    _paymentInfo.UpdateDebitCard(card);
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("NotAuthorized", "Home");
        }

        /// <summary>
        /// Creates a new subscription plan for a gym.
        /// </summary>
        /// <param name="bankAccount">The bank account information associated with the gym.</param>
        /// <returns>
        /// Redirects to the gym's index page if the plan creation is successful.
        /// Otherwise, redirects to the NotAuthorized action.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> CreatePlan(Models.Payment.BankAccount bankAccount)
        {
            if (bankAccount == null || string.IsNullOrEmpty(bankAccount.StripeBankId))
                return RedirectToAction("NotAuthorized");

            var sessionAccount = await _authenticationService.GetAccount("token", HttpContext.Session.GetString("AccessToken"));
            var gymBank = await _paymentInfo.GetGymBank((bankAccount.GymId == null) ? -1: (bankAccount.GymId.Value)) ;
            if (sessionAccount == null || gymBank == null || string.IsNullOrEmpty(gymBank.StripeBankId) || !_authorizationService.IsManager(sessionAccount, gymBank.Gym))
                return RedirectToAction("NotAuthorized");


            
            var newStripePlanId = await _stripeService.CreatePlan(
                bankAccount.GymSubscriptionName,
                (decimal)bankAccount.GymSubscriptionPrice
            );

            gymBank.GymSubscriptionName = bankAccount.GymSubscriptionName;
            gymBank.GymSubscriptionPrice = bankAccount.GymSubscriptionPrice;
            gymBank.StripePlanId = newStripePlanId;

            await _paymentInfo.UpdateGymBank(gymBank);
            

            return RedirectToAction("Index", "Gyms", new { gymId = gymBank.GymId });
        }
        /// <summary>
        /// Updates an existing subscription plan for a gym.
        /// </summary>
        /// <param name="bankAccount">The bank account information associated with the gym.</param>
        /// <returns>
        /// Redirects to the gym's index page if the plan update is successful.
        /// Otherwise, redirects to the NotAuthorized action.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> UpdatePlan(Models.Payment.BankAccount bankAccount)
        {
            if (bankAccount == null || string.IsNullOrEmpty(bankAccount.StripeBankId)) 
                return RedirectToAction("NotAuthorized");

            var sessionAccount = await _authenticationService.GetAccount("token", HttpContext.Session.GetString("AccessToken"));
            var gymBank = await _paymentInfo.GetGymBank((bankAccount.GymId == null) ? -1 : (bankAccount.GymId.Value));
            if (sessionAccount == null || gymBank == null || string.IsNullOrEmpty(gymBank.StripeBankId) || !_authorizationService.IsManager(sessionAccount, gymBank.Gym)) 
                return RedirectToAction("NotAuthorized");

            
            await _stripeService.DeletePlan(gymBank.StripePlanId);

            

            var newStripePlanId = await _stripeService.CreatePlan(
                bankAccount.GymSubscriptionName,
                (decimal)bankAccount.GymSubscriptionPrice
            );

            gymBank.GymSubscriptionName = bankAccount.GymSubscriptionName;
            gymBank.GymSubscriptionPrice = bankAccount.GymSubscriptionPrice;
            gymBank.StripePlanId = newStripePlanId;

            await _paymentInfo.UpdateGymBank(gymBank);

            
            var subscriptions = await _paymentInfo.GetSubscriptions((gymBank.GymId == null) ? -1 : (bankAccount.GymId.Value));

            foreach(var sub in subscriptions)
            {
                var details = new PaymentDetails
                {
                    Status = Status.SUBSCRIPTION,
                    Description = "update_subscription_" + gymBank.Gym.Name + "_" + sub.GetBuyer().StripeCustomerId,
                    PaymentDate = DateOnly.FromDateTime(DateTime.Now),
                    Buyable = new Models.Payment.Subscription
                    {
                        ClientDebitCard = sub.GetBuyer(),
                        GymBank = sub.GetSeller()
                    }
                };
                await _paymentService.CancelSubscription(sub.StripeSubscriptionId);
                sub.StripeSubscriptionId = await _paymentService.EnrollSubscription(details);
                await _paymentInfo.UpdateSubscription(sub);
            }

            return RedirectToAction("Index", "Gyms", new {gymId = gymBank.GymId});
            
        }
        /// <summary>
        /// Removes a subscription plan associated with a gym.
        /// </summary>
        /// <param name="bankAccountId">The ID of the bank account associated with the gym.</param>
        /// <returns>
        /// Redirects to the home page if the plan removal is successful.
        /// Otherwise, redirects to the NotAuthorized action.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> RemovePlan(int bankAccountId)
        {
            var bankAccount = await _paymentInfo.GetGymBankById(bankAccountId);
            var authorization = _authorizationService.isAuthorized((bankAccount.GymId == null) ? -1 : (bankAccount.GymId.Value), HttpContext.Session);
            var check = _stripeService.GetPlan(bankAccount.StripePlanId);
            if (authorization.Result.IsAuthorizedToEdit && check.Result == null)
            {
                var deletion = _stripeService.DeletePlan(bankAccount.StripePlanId);
                if (deletion.IsCompletedSuccessfully)
                {
                    bankAccount.StripePlanId = null;
                    _paymentInfo.UpdateGymBank(bankAccount);
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("NotAuthorized", "Home");
        }







        /*
        [HttpGet]
        public async Task<ActionResult> UpdateAccountLink(int gymId) {
            var gymBank = await _paymentInfo.GetGymBank(gymId);
            var refreshUrl = Url.Action("NotAuthorized", "Home", null, Request.Scheme);
            var returnUrl = Url.Action("Index", "Payments", null, Request.Scheme);
            if (String.IsNullOrEmpty(gymBank.StripeBankId)) { return RedirectToAction("NotAuthorized", "Home"); }
            var url = await _stripeService.UpdateAccountLink(gymBank.StripeBankId, refreshUrl, returnUrl);
            if (String.IsNullOrEmpty(url)) { return RedirectToAction("NotAuthorized","Home"); }
            return Redirect(url);
        }
        */




        /*
        public async Task<ActionResult> Index()
        {


            var customerId = await _stripeService.CreateCustomer
            (
                "FirstNameTeste",
                "LastName",
                "TesteEmail@gmail.com",
                "Corroios",
                "Rua Teste",
                "2855",
                "US"
            );
            var successUrl = Url.Action("Test", "Payments", null, Request.Scheme);
            var cancelUrl = Url.Action("NotAuthorized", "Home", null, Request.Scheme);
            

            var session = await _stripeService.CreatePaymentMethodSession
            (
                customerId,
                successUrl,
                cancelUrl
            );
            _logger.LogInformation(session);

            ViewData["SessionId"] = session;
            return View("PaymentMethod");
        }
        
        
        public async Task<ActionResult> MockPayment()
        {
            var details = new PaymentDetails
            {
                PaymentDetailsId = 1,
                Status = Status.PAYED,
                Description = "Compra teste!",
                PaymentDate = new DateOnly(2024,4 ,1),
                Buyable = new Cart
                {
                    ClientDebitCard = new DebitCard 
                    { 
                        StripeCustomerId = "cus_PlAlmfVUZR7R39",
                        StripePaymentMethodId = "pm_1OvgMnRseCm2355txbloh6st"
                    },
                    GymBank = new Models.Payment.BankAccount 
                    { 
                        StripeBankId = "acct_1Ovl7tRxsG4YewsC",
                    },
                    Items = new List<Models.Inventory.Item> 
                    { 
                        new Models.Inventory.Item 
                        {
                            Price = 1
                        },
                        new Models.Inventory.Item
                        {
                            Price = 2
                        },
                        new Models.Inventory.Item
                        {
                            Price = 1
                        },
                        new Models.Inventory.Item
                        {
                            Price = 1
                        },
                    }
                }
            };
            var res = await _paymentService.MakePayment(details);


            return Json ( res );
        }
        
        
        public async Task<ActionResult> MockSubscription()
        {
            var details = new PaymentDetails
            {
                PaymentDetailsId = 1,
                Status = Status.SUBSCRIPTION,
                Description = "Subscrição teste!",
                PaymentDate = new DateOnly(2024, 4, 1),
                Buyable = new Models.Payment.Subscription
                {
                    ClientDebitCard = new DebitCard
                    {
                        StripeCustomerId = "cus_PlAlmfVUZR7R39",
                        StripePaymentMethodId = "pm_1OvgMnRseCm2355txbloh6st"
                    },
                    GymBank = new Models.Payment.BankAccount
                    {
                        GymSubscriptionPrice = 5,
                        StripePlanId = await _stripeService.CreatePlan
                        (
                            "Subscrição teste1",
                            5
                        ),
                        StripeBankId = "acct_1Ovl7tRxsG4YewsC",
                    },
                }
            };
            var res = await _paymentService.EnrollSubscription(details);
            var subs = (Models.Payment.Subscription)details.Buyable;
            subs.StripeSubscriptionId = res;

            return Json(subs);
        }

        public async Task<ActionResult> GetPlan()
        {
            //var res = await _stripeService.GetProduct("prod_PlJClKQKhzuEyT");
            var res = await _stripeService.GetPlan("plan_PlJjMuuWr6VnrK");
            return Json(res);
        }
        public async Task<ActionResult> GetSubscription()
        {
            var res = await _stripeService.GetSubscription("sub_1OvmaSRseCm2355t4CWhSI63");
            return Json(res);
        }
        public async Task<ActionResult> CancelSubscription()
        {
            var res = await _paymentService.CancelSubscription("sub_1OvmaSRseCm2355t4CWhSI63");

            return Json(res);
        }
        
        
        public async Task<ActionResult> DeletePlan(string planId)
        {
            await _stripeService.DeletePlan(planId);
            
            return Json("");
        }
        public async Task<ActionResult> Delete()
        {
            var service = new AccountService();
            var list = await service.ListAsync();

            foreach ( var item in list)
            {
                _stripeService.DeleteAccount(item.Id);
            }
            return Json(list);
        }



        public async Task<ActionResult> Gym(string account)
        {

            var refreshUrl = Url.Action("NotAuthorized", "Home", null, Request.Scheme);
            var returnUrl = Url.Action("Index", "Payments", null, Request.Scheme);

            var url = await _stripeService.CreateAccountLink(account, refreshUrl, returnUrl);

            return new JsonResult(url);
        }

        public async Task<ActionResult> PaymentMethod(string customerId)
        {
            var successUrl = Url.Action("Test", "Payments", null, Request.Scheme);
            var cancelUrl = Url.Action("NotAuthorized", "Home", null, Request.Scheme);


            var session = await _stripeService.CreatePaymentMethodSession
            (
                customerId,
                successUrl,
                cancelUrl
            );

            ViewData["SessionId"] = session;
            return View("PaymentMethod");
        }

        public async Task<ActionResult> CreateTestData()
        {
            Dictionary<string, string> resust = new Dictionary<string, string>();
            var list = "123789".ToCharArray();
            for(var i=1; i<7; i++)
            {
                resust[("customerId" + list[i - 1]).ToString()] = await _stripeService.CreateCustomer
                (
                    "Test" + list[i - 1],
                    "Test" + list[i - 1],
                    "Teste" + list[i - 1] + "@gmail.com",
                    "Corroios",
                    "Rua Teste",
                    "2855",
                    "PT"
                );
            }

            resust["accountId1"] = await _stripeService.CreateAccountTestData
            (
                "GymTest1@gmail.com"
            );
            resust["accountId2"] = await _stripeService.CreateAccountTestData
            (
                "GymTest2@gmail.com"
            );

            return Json(resust);
        }

        public async Task<ActionResult> CreateTestPlans()
        {
            Dictionary<string, string> resust = new Dictionary<string, string>();

            resust["planId1"] = await _stripeService.CreatePlan("Subscrição GymTeste1", 30);
            resust["planId2"] = await _stripeService.CreatePlan("Subscrição GymTeste2", 25);
            var pIds = new List<string>
            {
                "pm_1Ow57fRseCm2355tph3WmrRE",
                "pm_1Ow58dRseCm2355tiOre9byR",
                "pm_1Ow59XRseCm2355te3uzVWWf",
                "pm_1Ow5AORseCm2355tQ25Jge06",
                "pm_1Ow5AyRseCm2355tgHg7cyRG",
                "pm_1Ow5BaRseCm2355thRNhZIui"
            };
            var cIds = new List<string>
            {
                "cus_PlbprNroroGeFr",
                "cus_Plbp64oawQ62uP",
                "cus_Plbp2N5j4PKj3t",
                "cus_PlbpHhHFVJJZgn",
                "cus_PlbpUx559W0bqL",
                "cus_Plbpb7N2yNwzgm"
            };
            var list = "123".ToCharArray();
            for (var i = 1; i < 4; i++)
            {
                var details = new PaymentDetails
                {
                    PaymentDetailsId = 1,
                    Status = Status.SUBSCRIPTION,
                    Description = "Subscrição GymTest1 para Test" + list[i - 1] + "!",
                    PaymentDate = new DateOnly(2024, 4, 1),
                    Buyable = new Models.Payment.Subscription
                    {
                        ClientDebitCard = new DebitCard
                        {
                            StripeCustomerId = cIds[i - 1],
                            StripePaymentMethodId = pIds[i-1]
                        },
                        GymBank = new Models.Payment.BankAccount
                        {
                            GymSubscriptionPrice = 30,
                            StripePlanId = resust["planId1"],
                            StripeBankId = "acct_1Ow4beRpgJmjBFH7",
                        },
                    }
                };
                
                resust[("subscriptionId" + list[i - 1]).ToString()] = await _paymentService.EnrollSubscription(details);

                
            }

            return Json(resust);
        }

        public async Task<ActionResult> GetPaymentIds()
        {
            Dictionary<string, string> resust = new Dictionary<string, string>();

            var list = "123789".ToCharArray();

            var ids = new List<string>
            {
                "cus_PlbprNroroGeFr",
                "cus_Plbp64oawQ62uP",
                "cus_Plbp2N5j4PKj3t",
                "cus_PlbpHhHFVJJZgn",
                "cus_PlbpUx559W0bqL",
                "cus_Plbpb7N2yNwzgm"
            };
            
            for (var i = 1; i < 7; i++)
            {
                var payments = await _stripeService.GetPaymentMethods(ids[i-1]);


                resust[("paymentId" + list[i - 1]).ToString()] = payments[0];
            }

            return Json(resust);
        }*/
    }
}
