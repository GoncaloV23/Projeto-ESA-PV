using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFitHub.Models.Account
{
    public class Manager : Account
    {
        public Manager() : base(AccountType.MANAGER) { }
        
        [ForeignKey("Id")]
        public int GymId { get; set; }
        public EasyFitHub.Models.Gym.Gym Gym { get; set; }
    }
}
