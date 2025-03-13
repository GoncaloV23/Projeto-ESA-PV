using Stripe;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFitHub.Models.Statistics
{
    public class PlatformStats
    {
        [Key]
        public int PlatFormStatsId { get; set; }
        public DateTime TheDate { get; set; } = DateTime.Now;

        public int GymCount { get; set; } = 0;
        public int UserCount { get; set; } = 0;
        public double AvgAge { get; set; } = 0;
        public IList<StatisticEntity?> SexRates { get; set; } = new List<StatisticEntity?>();
        public IList<StatisticEntity?> TopGyms { get; set; } = new List<StatisticEntity?>();

        public Dictionary<string, double> GetSexRatesDictionary() { return Utils.StatisticsUtils<string, double>.GetListToDictionary(SexRates); }
        public void SetSexRatesDictionary(Dictionary<string,double> dic) { SexRates = Utils.StatisticsUtils<string, double>.GetDictionaryToList(dic); }

        public Dictionary<string, int> GetTopGymsDictionary() { return Utils.StatisticsUtils<string, int>.GetListToDictionary(TopGyms); }
        public void SetTopGymsDictionary(Dictionary<string, int> dic) { TopGyms = Utils.StatisticsUtils<string, int>.GetDictionaryToList(dic); }
    }
}
