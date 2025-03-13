
using EasyFitHub.Data;

namespace EasyFitHub.Services
{
    /// <summary>
    /// AUTHOR: Rui Barroso
    /// Serviço usado para a criação automática de estatísticas no modelo
    /// </summary>
    public class StatisticsService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public static TimeSpan? DelayTime;

        public StatisticsService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        /// <summary>
        /// Método corrido para a chamada de criação de Estatisticas
        /// </summary>
        /// <param name="stoppingToken">Token usado para cancelar o serviço</param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await VerifyAndStoreStats(stoppingToken);

                TimeSpan delay;
                if (DelayTime == null)
                {
                    var nextMonth = DateTime.Today.AddMonths(1);
                    delay = nextMonth - DateTime.Now;
                }
                else
                {
                    delay = DelayTime.GetValueOrDefault();
                }

                await Task.Delay(delay, stoppingToken);
            }
        }
        /// <summary>
        /// Método usado para a criação de estatísticas
        /// </summary>
        /// <param name="stoppingToken">Token usado para cancelar o serviço</param>
        /// <returns></returns>
        private async Task VerifyAndStoreStats(CancellationToken stoppingToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<EasyFitHubContext>();
                StatisticsInfo statsInfo = new StatisticsInfo(dbContext);
                await statsInfo.CreateStats();
            }
        }
    }
}