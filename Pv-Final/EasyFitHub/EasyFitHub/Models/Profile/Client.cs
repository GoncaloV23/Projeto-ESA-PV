using EasyFitHub.Models.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFitHub.Models.Profile
{
    public class Client
    {
        public int ClientId { get; set; }
        [Required]
        public string Description { get; set; } = "No Description";
        [Required]
        public Gender Gender { get; set; }

        [ForeignKey("ClientDataId")]
        public int ClientDataId { get; set; }
        public ClientData Data { get; set; }

        [ForeignKey("BiometricsId")]
        public int BiometricsId { get; set; }
        public Biometrics Biometrics { get; set; }

        [ForeignKey("AccountId")]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
