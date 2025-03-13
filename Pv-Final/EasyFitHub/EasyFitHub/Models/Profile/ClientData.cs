using EasyFitHub.Models.Miscalenous;

namespace EasyFitHub.Models.Profile
{
    public class ClientData
    {
        public int ClientDataId { get; set; }
        public string Location { get; set; } = "Not set";
        //public string PostalCode { get; set; } = "Not set";
        public HubImage? Image { get; set; }
    }
}
