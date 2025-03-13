using Stripe;
using Stripe.Checkout;

namespace EasyFitHub.Services
{
    
    public class StripeService
    {
        /// <summary>
        /// AUTHOR: Rui Barroso
        /// Esta classe implementa Stripe
        /// Gerencia as operações e a gestão de Stripe Accounts,Customers,Payment Methods e Plans no Sistema 
        /// </summary>
        private readonly ILogger? _logger;
        public StripeService(ILogger? logger = null)
        {
            _logger = logger;
        }
        /// <summary>
        /// Cria uma sessão para a criação de um método de pagamento para um Customer 
        /// </summary>
        /// <param name="customerId">Id do Stripe Customer </param>
        /// <param name="urlSucess">URL de sucesso na criaçao</param>
        /// <param name="urlFail">URL de falha no criaçao</param>
        /// <returns>ID de metodo de pagamento</returns>
        public async Task<string?> CreatePaymentMethodSession(string customerId, string urlSucess, string urlFail) 
        {
            try
            {
                var options = new SessionCreateOptions
                {
                    Customer = customerId,
                    PaymentMethodTypes = new List<string>
                    {
                        "card",
                    },
                    Mode = "setup",
                    SuccessUrl = urlSucess,
                    CancelUrl = urlFail
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options);

                return session.Id;
            }
            catch (Exception ex)
            {
                printMessage($"Error in CreatePaymentMethodSession of StripeService: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Eliminar um método de pagamento
        /// </summary>
        /// <param name="paymentMethodId">Token de método de pagamento</param>
        /// <returns></returns>
        public async Task DeletePaymentMethod(string paymentMethodId)
        {
            try
            {
                var service = new PaymentMethodService();
                await service.DetachAsync(paymentMethodId);
            }
            catch (Exception ex)
            {
                printMessage($"Error in DeletePaymentMethod of StripeService: {ex.Message}");
            }
        }
        /// <summary>
        /// metodo de teste para criar uma Bank Account de teste
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<string?> CreateAccountTestData(string email) 
        {
            try
            {
                var options = new AccountCreateOptions
                {
                    Type = "standard",
                    Country = "PT",
                    Email = email,
                    ExternalAccount = new AccountBankAccountOptions
                    {
                        Country = "PT",
                        Currency = "eur",
                        AccountHolderName = "TESTE",
                        AccountHolderType = "individual",
                        AccountNumber = "PT50000201231234567890154", // Número de conta válido para teste
                    }
                };
                var service = new AccountService();
                var account = await service.CreateAsync(options);

                return account.Id;
            }
            catch (Exception ex)
            {
                printMessage($"Error in CreateAccountTestData of StripeService: {ex.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// Criar uma Stripe Account para um Manager
        /// </summary>
        /// <param name="name">Nome da empresa</param>
        /// <param name="email">Email da empresa</param>
        /// <param name="description">Descrição da empresa</param>
        /// <returns>Id  de StripeAccount</returns>
        //GymAccount
        public async Task<string?> CreateAccount(string name, string email, string description)
        {
            try
            {

            var options = new AccountCreateOptions
            {
                Type = "express",
                Country = "PT",
                BusinessType="company",
                BusinessProfile = new AccountBusinessProfileOptions
                {
                    Name = name,
                    SupportEmail = email,
                    ProductDescription = description,
                },
                Email = email,
                Company = new AccountCompanyOptions
                {
                    Name= name,
                },
                Capabilities = new AccountCapabilitiesOptions
                {
                    Transfers = new AccountCapabilitiesTransfersOptions { Requested = true },
                    CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true},
                },
                ExternalAccount = new AccountBankAccountOptions
                {
                    Country = "PT",
                    Currency = "eur",
                    AccountHolderName = "TESTE",
                    AccountHolderType = "individual",
                    AccountNumber = "PT50000201231234567890154",
                }
            };
            var service = new AccountService();
            var account = await service.CreateAsync(options);

            return account.Id;
            }
            catch (Exception ex)
            {
                printMessage($"Error in CreateAccount of StripeService: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Elimina uma Stripe Account
        /// </summary>
        /// <param name="accountId">Id da StripeAccount</param>
        /// <returns></returns>
        public async Task DeleteAccount(string accountId)
        {
            try
            {

                var service = new AccountService();
                await service.DeleteAsync(accountId);
            }
            catch (Exception ex)
            {
                printMessage($"Error in DeleteAccount of StripeService: {ex.Message}");
            }
        }
        /// <summary>
        /// Cria uma Sessão para fazer o Link de uma Stripe Account 
        /// </summary>
        /// <param name="accountId">Stripe Account Id</param>
        /// <param name="refreshUrl">The URL the user will be redirected to if the account link is expired, has been
        /// previously-visited, or is otherwise invalid</param>
        /// <param name="returnUrl">The URL that the user will be redirected to upon leaving or completing the linked flow.</param>
        /// <returns>Url de resposta da operação de Link</returns>
        public async Task<string?> CreateAccountLink(string accountId, string refreshUrl, string returnUrl)
        {
            try
            {
                var optionsLink = new AccountLinkCreateOptions
                {
                    Account = accountId,
                    RefreshUrl = refreshUrl,
                    ReturnUrl = returnUrl,
                    Type = "account_onboarding"
                };
                var serviceLink = new AccountLinkService();
                var res = await serviceLink.CreateAsync(optionsLink);

                return res.Url;
            }
            catch (Exception ex)
            {
                printMessage($"Error in CreateAccountLink of StripeService: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Cria uma Sessão para fazer o Link do update de uma Stripe Account 
        /// </summary>
        /// <param name="accountId">Stripe Account Id</param>
        /// <param name="refreshUrl">The URL the user will be redirected to if the account link is expired, has been
        /// previously-visited, or is otherwise invalid</param>
        /// <param name="returnUrl">The URL that the user will be redirected to upon leaving or completing the linked flow.</param>
        /// <returns>Url de resposta da operação de Link</returns>
        public async Task<string?> UpdateAccountLink(string accountId, string refreshUrl, string returnUrl)
        {
            try
            {

                var optionsLink = new AccountLinkCreateOptions
                {
                    Account = accountId,
                    RefreshUrl = refreshUrl,
                    ReturnUrl = returnUrl,
                    Type = "account_update"
                };
                var serviceLink = new AccountLinkService();
                var res = await serviceLink.CreateAsync(optionsLink);

                return res.Url;
            }
            catch (Exception ex)
            {
                printMessage($"Error in UpdateAccountLink of StripeService: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Cria um Stripe Customer para que este possa criar paymentMethods 
        /// </summary>
        /// <param name="FirstName">Nome do Customer</param>
        /// <param name="LastName">Sobre Nome do Customer</param>
        /// <param name="email">Email do Customer</param>
        /// <param name="PaymentAddress">Endereço de faturação do Customer</param>
        /// <returns>Id do novo Stripe Customer</returns>
        //Costumer
        public async Task<string?> CreateCustomer(string FirstName, string LastName,
            string email, string PaymentAddress)
        {
            try
            {

                var options = new CustomerCreateOptions
                {
                    Address = new AddressOptions
                    {
                        Line1 = PaymentAddress,
                    },
                    Email = email,
                    Name = FirstName + " " + LastName,

                };
                var service = new CustomerService();

                return (await service.CreateAsync(options)).Id;
            }
            catch (Exception ex)
            {
                printMessage($"Error in CreateCustomer of StripeService: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Atualiza informações de um Stripe customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="FirstName">Nome do Customer</param>
        /// <param name="LastName">Sobre Nome do Customer</param>
        /// <param name="email">Email do Customer</param>
        /// <param name="PaymentAddress">Endereço de faturação do Customer</param>
        /// <returns>Id do novo Stripe Customer</returns>
        public async Task<string?> UpdateCustomer(string customerId,
            string FirstName, string LastName,
            string email, string PaymentAddress)
        {
            try
            {

                var options = new CustomerUpdateOptions
                {
                    Address = new AddressOptions
                    {
                        Line1 = PaymentAddress,
                    },
                    Email = email,
                    Name = FirstName + " " + LastName,

                };
                var service = new CustomerService();

                return (await service.UpdateAsync(customerId, options)).Id;
            }
            catch (Exception ex)
            {
                printMessage($"Error in UpdateCustomer of StripeService: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Elimina um Stripe Customer 
        /// </summary>
        /// <param name="customerId">Id do Customer</param>
        /// <returns></returns>
        public async Task DeleteCustomer(string customerId)
        {
            try
            {

                var service = new CustomerService();
                await service.DeleteAsync(customerId);
            }
            catch (Exception ex)
            {
                printMessage($"Error in DeleteCustomer of StripeService: {ex.Message}");
            }
        }


        /// <summary>
        /// Devolve uma lista de Stripe Subscriptions vinculadas num plano
        /// </summary>
        /// <param name="planId">Id do plano</param>
        /// <returns>Lista de Subscription's efetuadas por Customers</returns>
        //Plan
        public async Task<List<Subscription>> GetSubscriptions(string planId)
        {
            try
            {

            var subscriptionsOptions = new SubscriptionListOptions
            {
                Plan = planId
            };
            var subscriptionsService = new SubscriptionService();
            var list = await subscriptionsService.ListAsync(subscriptionsOptions);

            return list.ToList();
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetSubscriptions of StripeService: {ex.Message}");
                return new List<Subscription>();
            }
        }
        /// <summary>
        /// Cria um plano de subscriçao
        /// </summary>
        /// <param name="name">Nome do ginásio</param>
        /// <param name="amount">Quantia Monetária</param>
        /// <returns>Id do Plan</returns>
        public async Task<string?> CreatePlan(string name, decimal amount)
        {
            try
            {

                var options = new PlanCreateOptions
                {
                    Amount = (long)(amount * 100),
                    Currency = "eur",
                    Interval = "month",
                    Product = new PlanProductOptions
                    {
                        Name = name,
                    }
                };

                var service = new PlanService();
                return (await service.CreateAsync(options)).Id;
            }
            catch (Exception ex)
            {
                printMessage($"Error in CreatePlan of StripeService: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Remove um Stripe Product
        /// usado no DeletePlan
        /// </summary>
        /// <param name="productToken">Id do Stripe Product</param>
        /// <returns></returns>
        public async Task DeleteProduct(string productToken)
        {
            try
            {
                var service = new ProductService();
                await service.DeleteAsync(productToken);
            }
            catch (Exception ex)
            {
                printMessage($"Error in DeleteProduct of StripeService: {ex.Message}");
            }
        }
        /// <summary>
        /// Remove um Stripe Plan do sistema
        /// 
        /// </summary>
        /// <param name="planToken">Id do Plano de subscrições</param>
        /// <returns></returns>
        public async Task DeletePlan(string planToken)
        {
            try
            {

                var res = (await GetPlan(planToken)).ProductId;

                var service = new PlanService();
                await service.DeleteAsync(planToken);
                await DeleteProduct(res);
            }
            catch (Exception ex)
            {
                printMessage($"Error in DeletePlan of StripeService: {ex.Message}");
            }
        }
        /// <summary>
        /// Devolve um Stripe Product
        /// </summary>
        /// <param name="productToken">Id do Product</param>
        /// <returns>A instância Product</returns>
        public async Task<Product?> GetProduct(string productToken)
        {
            try
            {

                var service = new ProductService();
                return await service.GetAsync(productToken);
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetProduct of StripeService: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Devolve um Plan
        /// </summary>
        /// <param name="planToken">Id do Plano</param>
        /// <returns>A instância Plan</returns>
        public async Task<Plan?> GetPlan(string planToken)
        {
            try
            {

                var service = new PlanService();
                return await service.GetAsync(planToken);
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetPlan of StripeService: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Devolve uma Stripe Subscription
        /// </summary>
        /// <param name="subscriptionToken">Id do Stripe Subscription</param>
        /// <returns>A instância Subscription</returns>
        public async Task<Subscription?> GetSubscription(string subscriptionToken)
        {
            try
            {

                var service = new SubscriptionService();
                return await service.GetAsync(subscriptionToken);
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetSubscription of StripeService: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Devolve um Stripe Customer
        /// </summary>
        /// <param name="customerId">Id do Customer</param>
        /// <returns>A instância Customer</returns>
        public async Task<Customer?> GetCustomer(string customerId)
        {
            try
            {

                var service = new CustomerService();
                var customer = await service.GetAsync(customerId);
                
                return customer;
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetCustomer of StripeService: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Devolve os Id's dos Payment Methods criados por um Customer
        /// </summary>
        /// <param name="customerId">Id do Customer</param>
        /// <returns>Lista de Id's relativos aos PaymentMethods</returns>
        public async Task<List<string>> GetPaymentMethods(string customerId)
        {
            try
            {

                var service = new CustomerService();
                var payments = (await service.ListPaymentMethodsAsync(customerId)).Select(pm => pm.Id).ToList<string>();

                return payments;
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetPaymentMethods of StripeService: {ex.Message}");
                return new List<string>();
            }
        }
        /// <summary>
        /// Atribui um payment method como Default
        /// </summary>
        /// <param name="customerId">Id do customer</param>
        /// <param name="paymentMethodId">Id do paymentMethod</param>
        /// <returns>O Customer com a nova propriedade </returns>
        public async Task<Customer?> SetDefaultPaymentMethods(string customerId, string paymentMethodId)
        {
            try
            {

                var customerService = new CustomerService();

                var options = new CustomerUpdateOptions
                {
                    InvoiceSettings = new CustomerInvoiceSettingsOptions
                    {
                        DefaultPaymentMethod = paymentMethodId
                    }
                };

                return await customerService.UpdateAsync(customerId, options);
            }
            catch (Exception ex)
            {
                printMessage($"Error in SetDefaultPaymentMethods of StripeService: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// retorna um Account atravez do seu Id
        /// </summary>
        /// <param name="accountId">Id da conta</param>
        /// <returns>A conta</returns>
        public async Task<Account?> GetAccount(string accountId)
        {
            try
            {
                var service = new AccountService();
                var account = await service.GetAsync(accountId);

                return account;
            }
            catch (Exception ex)
            {
                printMessage($"Error in GetAccount of StripeService: {ex.Message}");
                return null;
            }
        }





        private void printMessage(string message)
        {
            if (_logger == null)
                Console.WriteLine(message);
            else
                _logger.LogError(message);
        }
    }
}
