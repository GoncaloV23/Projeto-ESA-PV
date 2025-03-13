using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFitHub.Models.Gym
{
    public class GymRelation
    {
        [Key]
        public int GymRelationId { get; set; }

        [ForeignKey("GymClientId")]
        public int GymClientId { get; set; }
        public GymClient GymClient { get; set; }


        [ForeignKey("GymEmployeeId")]
        public int GymEmployeeId { get; set; }
        public GymEmployee GymEmployee { get; set; }
    }
}
