using Microsoft.Extensions.Primitives;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFitHub.Models.Statistics
{
    public class StatisticEntity
    {
        [Key]
        public int Id { get; set; }

        [NotMapped]
        private string _StringValue { get; set; } = null;
        public string StringValue { get=> _StringValue; set {
                if (String.IsNullOrEmpty(value)) 
                    _StringValue = EntityKey + ", " + EntityValue;
                else
                {
                    var vals = value.Split(", ");
                    if (vals.Length == 2)
                    {
                        EntityKey = vals[0].Trim();
                        EntityValue = vals[1].Trim();
                        _StringValue = EntityKey + ", " + EntityValue;
                    } 
                }
            }
        }
        [NotMapped]
        public string EntityKey { get; set; } = "N/A";

        [NotMapped]
        public string EntityValue { get; set; } = "N/A";
    }
}
