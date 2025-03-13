using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using EasyFitHub.Models.Profile;
using System.ComponentModel;
using EasyFitHub.Models.Account;

namespace EasyFitHub.Models.Gym
{
    public class GymEmployee
    {
        public GymEmployee() { EnrollmentDate = DateTime.Now; }
        [Key]
        public int GymEmployeeId { get; set; }
        [DisplayName("Enrollment Date")]
        public DateTime EnrollmentDate { get; set; }
        public Role Role { get; set; }

        [ForeignKey("ClientId")]
        public int ClientId { get; set; }
        public Client Client { get; set; }

        public IList<GymRelation> GymClients { get; set; } = new List<GymRelation>();

        [ForeignKey("Id")]
        public int GymId { get; set; }
        public Gym Gym { get; set; }
    }
}