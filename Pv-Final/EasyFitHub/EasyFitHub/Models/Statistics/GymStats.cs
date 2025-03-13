using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EasyFitHub.Models.Statistics
{
    public class GymStats
    {
        [Key]
        public int GymStatsId { get; set; }
        public DateTime TheDate { get; set; } = DateTime.Now;

        public int ClientCount { get; set; } = 0;
        public int PTCount { get; set; } = 0;
        public int NutricionistCount { get; set; } = 0;
        public int SecretaryCount { get; set; } = 0;
        public int ShopItemCount { get; set; } = 0;
        public int MachineCount { get; set; } = 0;
        public IList<StatisticEntity?> AgeRates { get; set; } = new List<StatisticEntity?>();
        public IList<StatisticEntity?> SexRates { get; set; } = new List<StatisticEntity?>();


        [ForeignKey("Id")]
        public int? GymId { get; set; }
        public EasyFitHub.Models.Gym.Gym? Gym { get; set; }

        public Dictionary<int, double> GetAgeRatesDictionary() { return Utils.StatisticsUtils<int, double>.GetListToDictionary(AgeRates); }
        public void SetAgeRatesDictionary(Dictionary<int, double> dic) { AgeRates = Utils.StatisticsUtils<int, double>.GetDictionaryToList(dic); }
        public Dictionary<string, double> GetSexRatesDictionary() { return Utils.StatisticsUtils<string, double>.GetListToDictionary(SexRates); }
        public void SetSexRatesDictionary(Dictionary<string, double> dic) { SexRates = Utils.StatisticsUtils<string, double>.GetDictionaryToList(dic); }
    }
}
