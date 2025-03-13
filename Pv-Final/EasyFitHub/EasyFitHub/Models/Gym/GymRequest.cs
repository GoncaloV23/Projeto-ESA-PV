using EasyFitHub.Models.Profile;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFitHub.Models.Gym
{
    public class GymRequest
    {
        [Key]
        public int RequestId { get; set; }
        [ForeignKey("ClientId")]
        public int ClientId { get; set; }
        public Client Client { get; set; }

        [ForeignKey("Id")]
        public int GymId { get; set; }
        public Gym Gym { get; set; }
    }
}
