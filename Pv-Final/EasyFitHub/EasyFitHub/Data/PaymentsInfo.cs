using EasyFitHub.Models.Account;
using EasyFitHub.Models.Gym;
using EasyFitHub.Models.Payment;
using EasyFitHub.Models.Profile;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Common;
using NuGet.Versioning;
using Stripe;
using System;
using System.Reflection.Metadata.Ecma335;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace EasyFitHub.Data
{
    public class PaymentsInfo
    {
        /// <summary>
        /// AUTHOR:André P.
        /// A classe lida com as operações CRUD no contexto
        /// As operações são efetuadas no Modelo Payment principalmente
        /// Porém também são feitas operações nos Modelos:
        ///     Account, Gym, Profile
        /// </summary>
        private readonly EasyFitHubContext _context;
        private readonly ILogger? _logger;

        public PaymentsInfo(EasyFitHubContext context, ILogger? logger = null)
        {
            _context = context;
            _logger = logger;
        }
        /// <summary>
        /// Cria um BankAccount no contexto
        /// </summary>
        /// <param name="bankAccount">instancia para a adiçao no contexto</param>
        /// <returns> NULL se já existe um bankAccount no contexto ou se a adição assíncrona falhar
        /// Se tudo correr bem é retornado o bankAccount</returns>
        public async Task<Models.Payment.BankAccount?> CreateBankAccount(Models.Payment.BankAccount bankAccount)
        {
            try 
            { 
                var query=await _context.BankAccounts.Include(b=>b.Gym).SingleOrDefaultAsync(b=>b.StripeBankId == bankAccount.StripeBankId);
                if (query != null) { return null; }
                var addition = await _context.BankAccounts.AddAsync(bankAccount);
                if (addition.State == EntityState.Added)
                {
                    await _context.SaveChangesAsync();
                    return addition.Entity;
                }
                return null;
            }catch(Exception ex)
            {
                printMessage($"Error in CreateBankAccount of PaymentsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// retorna  um BankAccount associado a um gym
        /// </summary>
        /// <param name="gymId">Id do Gym</param>
        /// <returns>a instancia de BankAccount; NULL em caso de erro</returns>
        public async Task<Models.Payment.BankAccount?> GetGymBank(int gymId)
        {
            try
            {
                return await _context.BankAccounts.Include(b => b.Gym).SingleOrDefaultAsync(b => b.Gym.Id == gymId);
            }catch(Exception ex)
            {
                printMessage($"Error in GetGymBank of PaymentsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// retorna um BankAccount associado a um gym pelo seu Id
        /// </summary>
        /// <param name="bankId"> Id do BankAccount</param>
        /// <returns>a instancia de BankAccount; NULL em caso de erro</returns>
        public async Task<Models.Payment.BankAccount?> GetGymBankById(int bankId)
        {
            try
            {
                return await _context.BankAccounts.Include(b => b.Gym).SingleOrDefaultAsync(b => b.BankAccountId == bankId);
            }catch(Exception ex)
            {
                printMessage($"Error in GetGymBankById of PaymentsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Cria um DebitCard no contexto
        /// </summary>
        /// <param name="debitCard">A instancia de DebitCard</param>
        /// <returns>A instancia em caso de succeso; NULL em caso de erro</returns>
        public async Task<DebitCard?> CreateDebitCard(DebitCard debitCard)
        {
            try { 
                var card = (await _context.DebitCard.AddAsync(debitCard)).Entity;
            

                await _context.SaveChangesAsync();

                return card;
            }catch(Exception ex)
            {
                printMessage($"Error in CreateDebitCard of PaymentsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Atualiza um DebitCard no contexto
        /// </summary>
        /// <param name="debitCard">A instancia DebitCard Atualizada</param>
        /// <returns>A Instancia DebitCard em caso de sucesso; NULL em caso de erro</returns>
        public async Task<DebitCard?> UpdateDebitCard(DebitCard debitCard)
        {
            try { 
                var query = await _context.DebitCard.Include(d => d.Client).FirstAsync(d => d.ClientId== debitCard.ClientId);
                if (query == null) { return null; }
                query.StripePaymentMethodId = debitCard.StripePaymentMethodId;
                query.StripeCustomerId = debitCard.StripeCustomerId;
                await _context.SaveChangesAsync();
                return query;
            }catch(Exception ex)
            {
                printMessage($"Error in UpdateDebitCard of PaymentsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Devolve um Client do contexto pelo seu AccountId
        /// </summary>
        /// <param name="accountId">Id da Account associada ao Client</param>
        /// <returns>A instancia Client; NULL em caso de Erro</returns>
        public async Task<Client?> GetClient(int accountId)
        {
            try { 
                return await _context.Client.Include(c=>c.Biometrics).Include(c=>c.Data).Include(c=>c.User).FirstAsync(c=>c.User.AccountId==accountId);
            }catch(Exception ex)
            {
                printMessage($"Error in GetClient of PaymentsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// devolce o DebitCard associado ao um Client atraves do seu User:AccountId
        /// </summary>
        /// <param name="userId">Id de um Account:User</param>
        /// <returns>a instancia DebitCard; NULL em caso de Erro</returns>
        public async Task<DebitCard?> GetClientCard(int userId)
        {
            try { 
                return await _context.DebitCard.Include(d=>d.Client).ThenInclude(c => c.User).SingleOrDefaultAsync(d=>d.Client.UserId == userId);
            }catch(Exception ex)
            {
                printMessage($"Error in GetClientCard of PaymentsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// atualiza um BankAccount no contexto
        /// </summary>
        /// <param name="bankAccount">a instancia BankAccount atualizada</param>
        /// <returns>a instancia BankAccount atualizada; NULL em caso de erro</returns>
        public async Task<Models.Payment.BankAccount?> UpdateGymBank(Models.Payment.BankAccount bankAccount)
        {
            try { 
                var query = await _context.BankAccounts.Include(b=>b.Gym).FirstAsync(b=>b.BankAccountId==bankAccount.BankAccountId);
                if(query == null) { return null; }
                query.GymSubscriptionPrice = bankAccount.GymSubscriptionPrice;
                query.StripePlanId = bankAccount.StripePlanId;
                query.StripeBankId = bankAccount.StripeBankId;
                await _context.SaveChangesAsync();
                return query;
            }catch(Exception ex)
            {
                printMessage($"Error in UpdateGymBank of PaymentsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Cria uma instancia PaymentDetails no contexto
        /// </summary>
        /// <param name="details">A instancia PaymentDetails</param>
        /// <returns>a instancia PaymentDetails; NULL em caso de erro</returns>
        public async Task<PaymentDetails?> AddPaymentDetails(PaymentDetails details)
        {
            try { 
                var chekc = await _context.PaymentDetails
                    .Include(p=>p.Buyable)
                    .FirstOrDefaultAsync(p=>p.PaymentDetailsId==details.PaymentDetailsId);
                if (chekc != null) { return null; }
                var adition =await _context.PaymentDetails.AddAsync(details);

                await _context.SaveChangesAsync();
                return adition.Entity;
            }catch(Exception ex)
            {
                printMessage($"Error in AddPaymentDetails of PaymentsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Devolve uma instancia PaymentDetails medido do seu Id
        /// </summary>
        /// <param name="detailsId">Id do PaymentDetails</param>
        /// <returns>A instancia de PaymentDetails; NULL em caso de erro</returns>
        public async Task<PaymentDetails?> GetPaymentDetails(int detailsId)
        {
            try { 
                var details = await _context.PaymentDetails
                    .Include(d => d.Buyable).ThenInclude(b => b.GymBank).ThenInclude(gb => gb.Gym)
                    .Include(d => d.Buyable).ThenInclude(b => b.ClientDebitCard).ThenInclude(cc => cc.Client)
                    .SingleOrDefaultAsync(p => p.PaymentDetailsId == detailsId);
                if (details == null) return null;
                if (details.Buyable.BuyableType == BuyableType.CART)
                {
                    var cart = (Cart)details.Buyable;
                    await _context.Entry(cart)
                        .Collection(c => c.Items)
                        .Query()
                        .Include(ci => ci.Item)
                        .LoadAsync();
                    details.Buyable = cart;
                }

                return details;
            }catch(Exception ex)
            {
                printMessage($"Error in GetPaymentDetails of PaymentsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Cria um Buyable:Subscription no contexto
        /// </summary>
        /// <param name="subscription">A instancia Subscription</param>
        /// <returns>a instancia Subscription; NULL em caso de erro</returns>
        public async Task<Models.Payment.Subscription?>CreateSubscription(Models.Payment.Subscription subscription)
        {
            try { 
                var sub = await _context.Buyables.AddAsync(subscription);

                await _context.SaveChangesAsync();

                return sub.Entity as Models.Payment.Subscription;
            }catch(Exception ex)
            {
                printMessage($"Error in CreateSubscription of PaymentsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// atualiza um Subscription no contexto
        /// </summary>
        /// <param name="subscription">A instancia nova de Subscription</param>
        /// <returns>A instancia nova de Subscription; NULL em caso de erro</returns>
        public async Task<Models.Payment.Subscription?> UpdateSubscription(Models.Payment.Subscription subscription)
        {
            try{
                var dbSub = await GetSubscription(subscription.BuyableId);

                dbSub.StripeSubscriptionId = subscription.StripeSubscriptionId;

                await _context.SaveChangesAsync();

                return dbSub;
            }catch(Exception ex)
            {
                printMessage($"Error in UpdateSubscription of PaymentsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// devolve uma instancia Subscription
        /// </summary>
        /// <param name="subcriptionId">O Id de Subscription</param>
        /// <returns>A instancia Subscription</returns>
        public async Task<Models.Payment.Subscription?> GetSubscription(int subcriptionId)
        {
            try { 
                return await _context.Buyables.OfType<Models.Payment.Subscription>().Include(b=>b.GymBank).Include(b=>b.ClientDebitCard).SingleOrDefaultAsync(b => b.BuyableId == subcriptionId);
            }catch(Exception ex)
            {
                printMessage($"Error in GetSubscription of PaymentsInfo: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Devolve uma instância PaymentDetails
        /// </summary>
        /// <param name="gymId">Id do ginásio</param>
        /// <param name="userId">Id do User vinculado ao ginásio</param>
        /// <returns></returns>
        public async Task<PaymentDetails?> GetSubscriptionPaymentDetail(int gymId, int userId)
        {
            try
            {
                return await _context.PaymentDetails
                .Include(d => d.Buyable).ThenInclude(b => b.GymBank)
                .Include(d => d.Buyable).ThenInclude(b => b.ClientDebitCard).ThenInclude(cc => cc.Client)
                .SingleOrDefaultAsync
                (d =>
                    d.Buyable.GymBank.GymId == gymId &&
                    d.Buyable.ClientDebitCard.Client.UserId == userId
                );
            }catch(Exception ex)
            {
                printMessage($"Error in GetSubscriptionPaymentDetail of PaymentsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Devolve uma instancia PaymentDetails consoante o Status da mesma
        /// </summary>
        /// <param name="gymId">Id do ginásio</param>
        /// <param name="userId">Id do User vinculado ao ginásio</param>
        /// <param name="status">O Status pretendido ;Default: Status.SUBSCRIPTION</param>
        /// <returns>A instancia de PaymentDetails;NULL em caso de erro</returns>
        public async Task<PaymentDetails?> GetSubscriptionPaymentDetailWithStatus(int gymId, int userId, Status status = Status.SUBSCRIPTION)
        {
            try { 
                return await _context.PaymentDetails
                    .Include(d => d.Buyable).ThenInclude(b => b.GymBank)
                    .Include(d => d.Buyable).ThenInclude(b => b.ClientDebitCard).ThenInclude(cc => cc.Client)
                    .SingleOrDefaultAsync
                    (   d =>
                        d.Buyable.BuyableType == BuyableType.SUBSCRIPTION &&
                        d.Status == status &&
                        d.Buyable.GymBank.GymId == gymId && 
                        d.Buyable.ClientDebitCard.Client.UserId == userId
                    );
            }catch(Exception ex)
            {
                printMessage($"Error in GetSubscriptionPaymentDetailWithStatus of PaymentsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Cria um PaymentDetails no contexto
        /// </summary>
        /// <param name="details">A instância de PaymentDetails</param>
        /// <returns>A instancia PaymentDetails; NULL em caso de Erro</returns>
        public async Task<PaymentDetails?> CreatePaymentDetail(PaymentDetails details)
        {
            try { 
                var ent = await _context.PaymentDetails.AddAsync(details);

                await _context.SaveChangesAsync();

                return ent.Entity;
            }catch(Exception ex)
            {
                printMessage($"Error in CreatePaymentDetail of PaymentsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Atualiza um PaymentDetails consoante o seu Id
        /// </summary>
        /// <param name="detailsId">O Id do instância a ser modificada</param>
        /// <param name="detaisl">A nova instância</param>
        /// <returns>A nova instância PaymentDetails; NULL em caso de erro</returns>
        public async Task<PaymentDetails?> UpdatePaymentDetail(int detailsId, PaymentDetails detaisl)
        {
            try { 
                var dbSub = await _context.PaymentDetails.SingleOrDefaultAsync(b => b.PaymentDetailsId == detailsId);
                if (dbSub == null) return null;

                dbSub.Description = detaisl.Description;
                dbSub.Status = detaisl.Status;

                await _context.SaveChangesAsync();

                return dbSub;
            }catch(Exception ex)
            {
                printMessage($"Error in UpdatePaymentDetail of PaymentsInfo: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Atualiza uma instancia PaymentDetails consoante o Id do Buyable:Subscription associado
        /// </summary>
        /// <param name="subscriptionId">Id da Subscription associado ao PaymentDetails</param>
        /// <param name="detaisl">O novo PaymentDetails</param>
        /// <returns>A nova instância PaymentDetails; Null em caso de erro</returns>
        public async Task<PaymentDetails?> UpdatePaymentDetailWithSubscription(int subscriptionId, PaymentDetails detaisl)
        {
            try
            {
                var dbSub = await _context.PaymentDetails.SingleOrDefaultAsync(b => b.BuyableId == subscriptionId);
                if (dbSub == null) return null;

                dbSub.Description = detaisl.Description;
                dbSub.Status = detaisl.Status;

                await _context.SaveChangesAsync();

                return dbSub;
            }
            catch (Exception ex)
            {
                printMessage($"Error in UpdatePaymentDetailWithSubscription of PaymentsInfo: {ex.Message}");
                return null;
            } 
        }
        /// <summary>
        /// Remove uma instância Subscription do contexto atravez do seu Id
        /// </summary>
        /// <param name="subscriptionId">Id da subscription</param>
        /// <returns>A instância removida; NULL em caso de erro</returns>
        public async Task<PaymentDetails?> DeletePaymentDetail(int subscriptionId)
        {
            try { 
                var query = await _context.PaymentDetails.FirstAsync(b => b.BuyableId == subscriptionId);
                if (query == null) return null;
                var delettion = _context.PaymentDetails.Remove(query);
                if(delettion.State == EntityState.Deleted)
                {
                    await _context.SaveChangesAsync();
                    return query;
                }return null;

            }catch(Exception ex)
            {
                printMessage($"Error in DeletePaymentDetail of PaymentsInfo: {ex.Message}");
                return null;
            }
        }/// <summary>
        /// Devolve o Account:Manager (owner) atravez do Id do Gym associado
        /// </summary>
        /// <param name="gymId">Id do Gym associado ao Manager</param>
        /// <returns>A instania Manager; NULL em caso de erro</returns>
        public async Task<Models.Account.Account?> GetGymOwner(int gymId)
        {
            try { 
                var query = await _context.Gym.FirstAsync(g=>g.Id == gymId);
                if(query != null)
                {
                    var acc = await _context.Account.OfType<Manager>().FirstAsync(a => a.GymId == gymId);
                    return acc;
                }
                return null;
            }catch(Exception ex)
            {
                printMessage($"Error in GetGymOwner of PaymentsInfo: {ex.Message}");
                return null;
            }

        }/// <summary>
        /// 
        /// </summary>
        /// <param name="gymId"></param>
        /// <returns></returns>
        public async Task<Gym?> GetGym(int gymId)
        {
            try { 
                return await _context.Gym.SingleOrDefaultAsync(g => g.Id == gymId);
            }catch(Exception ex)
            {
                printMessage($"Error in GetGym of PaymentsInfo: {ex.Message}");
                return null;
            }

        }
        /// <summary>
        /// Remove um BankAccount através do Id do ginásio
        /// </summary>
        /// <param name="gymId">Id do ginásio</param>
        /// <returns>A instância removida; NULL em caso de erro</returns>
        public async Task<Models.Payment.BankAccount?> DeleteBank(int gymId)
        {
            try { 
                var gym = await GetGym(gymId);
                var bankAcc = await _context.BankAccounts.Include(b => b.Gym).FirstAsync(b => b.GymId == gymId);
                if (gym == null || bankAcc == null) return null;
                var deletion = _context.BankAccounts.Remove(bankAcc);
                if (deletion.State == EntityState.Deleted) { await _context.SaveChangesAsync(); return bankAcc; }
                return null;
            }catch(Exception ex)
            {
                printMessage($"Error in DeleteBank of PaymentsInfo: {ex.Message}");
                return null;
            }
}
        /// <summary>
        /// Devolve uma lista de Subscriptions vinculadas a um ginásio
        /// </summary>
        /// <param name="gymId">Id do Ginásio</param>
        /// <returns>Lista com as Subcriptions do ginásio; Lista vazia em caso de erro</returns>
        public async Task<List<Models.Payment.Subscription>> GetSubscriptions(int gymId)
        {
            try
            {
                return await _context.Buyables.OfType<Models.Payment.Subscription>()
                    .Include(s => s.GymBank)
                    .Include(s => s.ClientDebitCard)
                    .Where(s => s.GymBank.GymId == gymId)
                    .ToListAsync();
            }catch(Exception ex)
            {
                printMessage($"Error in GetSubscriptions of PaymentsInfo: {ex.Message}");
                return new List<Models.Payment.Subscription>();
            }
        }


        /// <summary>
        /// Função auxiliar para debugging, imprime na consola uma mensagem
        /// </summary>
        /// <param name="message">a mensagem</param>


        private void printMessage(string message)
        {
            if (_logger == null)
                Console.WriteLine($"\n\n\n\n{message}\n\n\n\n");
            else
                _logger.LogError($"\n\n\n\n{message}\n\n\n\n");
        }
    }
}

