using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using EasyFitHub.Models.Profile;
using System.ComponentModel;
using EasyFitHub.Models.Account;

namespace EasyFitHub.Models.Gym
{
    public class GymClient
    {
        public GymClient() { EnrollmentDate = DateTime.Now; }
        [Key]
        public int GymClientId { get; set; }
        [DisplayName("Enrollment Date")]
        public DateTime? EnrollmentDate { get; set; }
        public Role Role { get; set; } = Role.CLIENT;

        [ForeignKey("ClientId")]
        public int ClientId { get; set; }
        public Client Client { get; set; }


        [ForeignKey("Id")]
        public int GymId { get; set; }
        public Gym Gym { get; set; }


        public IList<GymRelation> GymEmployees { get; set; } = new List<GymRelation>();
    }
}
