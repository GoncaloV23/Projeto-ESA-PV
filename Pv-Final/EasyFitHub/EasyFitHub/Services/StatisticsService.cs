
using EasyFitHub.Data;

namespace EasyFitHub.Services
{
    /// <summary>
    /// AUTHOR: Rui Barroso
    /// Servi�o usado para a cria��o autom�tica de estat�sticas no modelo
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
        /// M�todo corrido para a chamada de cria��o de Estatisticas
        /// </summary>
        /// <param name="stoppingToken">Token usado para cancelar o servi�o</param>
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
        /// M�todo usado para a cria��o de estat�sticas
        /// </summary>
        /// <param name="stoppingToken">Token usado para cancelar o servi�o</param>
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