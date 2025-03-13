using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using EasyFitHub.Models.Inventory;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using EasyFitHub.Models.Profile;

namespace EasyFitHub.Models.Statistics
{
    public class ClientStats
    {
        [Key]
        public int ClientStatsId { get; set; }
        public DateTime TheDate { get; set; } = DateTime.Now;

        public double Weigth { get; set; }
        public double Height { get; set; }
        public double FatMass { get; set; }
        public double LeanMass { get; set; }
        public double BodyMassIndex { get; set; }
        public double VisceralFat { get; set; }

        [ForeignKey("ClientId")]
        public int? ClientId {  get; set; }
        public Client? Client {  get; set; }
    }
}
