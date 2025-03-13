using EasyFitHub.Models.Profile;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFitHub.Models.Statistics
{
    public class EmployeeStats
    {
        [Key]
        public int EmployeeStatsId { get; set; }
        public DateTime TheDate { get; set; } = DateTime.Now;

        public int ClientCount { get; set; } = 0;
        public IList<StatisticEntity?> UserSexRates { get; set; } = new List<StatisticEntity?>();
        public IList<StatisticEntity?> UserAgeRates { get; set; } = new List<StatisticEntity?>();


        [ForeignKey("ClientId")]
        public int? ClientId { get; set; }
        public Client? Client { get; set; }

        public Dictionary<string, double> GetUserSexRatesDictionary() { return Utils.StatisticsUtils<string, double>.GetListToDictionary(UserSexRates); }
        public void SetUserSexRatesDictionary(Dictionary<string, double> dic) { UserSexRates = Utils.StatisticsUtils<string, double>.GetDictionaryToList(dic); }
        public Dictionary<int, double> GetUserAgeRatesDictionary() { return Utils.StatisticsUtils<int, double>.GetListToDictionary(UserAgeRates); }
        public void SetUserAgeRatesDictionary(Dictionary<int, double> dic) { UserAgeRates = Utils.StatisticsUtils<int, double>.GetDictionaryToList(dic); }
    }
}
