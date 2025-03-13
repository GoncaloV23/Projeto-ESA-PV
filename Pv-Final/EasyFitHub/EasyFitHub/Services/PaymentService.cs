using EasyFitHub.Models.Payment;
using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Stripe;
using System.Numerics;

namespace EasyFitHub.Services
{
    public class PaymentService
    {
        /// <summary>
        /// AUTHOR: Rui Barroso
        /// Destinado a efetuar pagamentos e subscrições usando o stripe api.
        /// </summary>
        public async Task<bool> MakePayment(PaymentDetails details)
        {
            try
            {
                var amount = (long)(details.Buyable.GetCost() * 100);
                var paymentOptions = new PaymentIntentCreateOptions
                {
                    Amount = amount,
                    Currency = "eur",
                    Customer = details.Buyable.GetBuyer().StripeCustomerId,
                    PaymentMethod = details.Buyable.GetBuyer().StripePaymentMethodId,
                    //ConfirmationMethod = "automatic",
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    { 
                        Enabled = true,
                        AllowRedirects = "never"
                    },
                    TransferData = new PaymentIntentTransferDataOptions
                    { 
                        Amount = amount,
                        Destination = details.Buyable.GetSeller().StripeBankId
                    },
                    Confirm = true
                };

                var paymentService = new PaymentIntentService();
                var paymentIntent = await paymentService.CreateAsync(paymentOptions);

                if (paymentIntent.Status == "succeeded")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string? MakeReceipt(PaymentDetails details)
        {
            try
            {
                string receipt = $"Payment ID: {details.PaymentDetailsId}\n" +
                     $"Payment Date: {details.PaymentDate.ToString("yyyy-MM-dd")}\n" +
                     $"Description: {details.Description}\n" +
                     $"Status: {details.Status}\n" +
                     $"Buyable ID: {details.BuyableId}\n" +
                     $"Buyer: {details.Buyable.GetBuyer()}" +
                     $"Seller: {details.Buyable.GetSeller()}" +
                     $"Cost: {details.Buyable.GetCost()}";

                return receipt;

            } catch(Exception ex) 
            {
                return null;
            }
            
        }

        public async Task<string?> EnrollSubscription(PaymentDetails details) 
        {
            try
            {
                var subscriptionOptions = new SubscriptionCreateOptions
                {
                    Customer = details.Buyable.GetBuyer().StripeCustomerId,
                    Items = new List<SubscriptionItemOptions>
                    {
                        new SubscriptionItemOptions
                        {
                            Plan = details.Buyable.GetSeller().StripePlanId,
                        },
                    },
                    TransferData = new SubscriptionTransferDataOptions
                    {
                        Destination = details.Buyable.GetSeller().StripeBankId,
                    },
                    DefaultPaymentMethod = details.Buyable.GetBuyer().StripePaymentMethodId,
                };

                var subscriptionService = new SubscriptionService();
                var subscription = await subscriptionService.CreateAsync(subscriptionOptions);

                if(subscription.Status == "active")return subscription.Id;
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<bool> CancelSubscription(string subscriptionId) 
        {
            var service = new SubscriptionService();
            var options = new SubscriptionCancelOptions
            {
                InvoiceNow = true, 
                Prorate = true 
            };

            try
            {
                var canceledSubscription = await service.CancelAsync(subscriptionId, options);
                if (canceledSubscription.Status == "canceled")
                {
                    return true;
                }
            }catch(Exception ex) { return false; }
            
            return false;
        }
    }
}
