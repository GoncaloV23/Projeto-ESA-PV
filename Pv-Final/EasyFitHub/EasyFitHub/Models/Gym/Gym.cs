using EasyFitHub.Models.Account;
using EasyFitHub.Models.Inventory;
using EasyFitHub.Models.Miscalenous;
using EasyFitHub.Models.Profile;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EasyFitHub.Models.Gym
{
    public class Gym
    {
        public Gym() { RegisterDate = DateTime.Now; }
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Gym name is required.")]
        [MaxLength(100, ErrorMessage = "Gym name is can not have more then 100 charecters.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Gym description is required.")]
        [MaxLength(1000, ErrorMessage = "Gym description is can not have more then 1000 charecters.")]
        public string Description { get; set; } = "No description!";
        [Required(ErrorMessage = "Gym location is required.")]
        [MaxLength(100, ErrorMessage = "Gym location is can not have more then 100 charecters.")]
        public string Location { get; set; } = "No location!";
        [DisplayName("Confirmation Status")]
        public bool IsConfirmed { get; set; } = false;
        [DisplayName("Registration Date")]
        [DataType(DataType.Date)]
        public DateTime RegisterDate { get; set; }
        public ICollection<HubImage> Images { get; set; } = new List<HubImage>();

        //Users
        public ICollection<GymEmployee> GymEmployees { get; set; } = new List<GymEmployee>();
        public ICollection<GymClient> GymClients { get; set; } = new List<GymClient>();
        public ICollection<GymRequest> Requests { get; set; } = new List<GymRequest>();

        //Inventory
        public ICollection<GymMachine> Machines { get; set; } = new List<GymMachine>();
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
