using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EasyFitHub.Models.Profile
{
    public class Biometrics
    {
        public int BiometricsId { get; set; }

        [Range(0, 600, ErrorMessage = "Weight must be between 0.01 and 600.")]
        public double Weigth { get; set; }

        [Range(0, 3, ErrorMessage = "Heigth must be between 0.01 and 3.")]
        public double Height { get; set; }

        [DisplayName("Water Percentage")]
        [Range(0, 100, ErrorMessage = "Water Percentage must be between 0 and 100.")]
        public double WaterPercentage { get; set; }

        [Range(0, 500, ErrorMessage = "Fat Mass must be between 0 and 500.")]
        [DisplayName("Fat Mass")]
        public double FatMass { get; set; }

        [Range(0, 500, ErrorMessage = "Lean Mass must be between 0 and 500.")]
        [DisplayName("Lean Mass")]
        public double LeanMass { get; set; }

        [Range(0, 100, ErrorMessage = "Body-Mass Index must be between 0.01 and 1000.")]
        [DisplayName("Body-Mass Index")]
        public double BodyMassIndex { get; set; }

        [Range(0, 110, ErrorMessage = "Metabolic Age must be between 0 and 110.")]
        [DisplayName("Metabolic Age")]
        public int MetabolicAge { get; set; }

        [Range(0, 100, ErrorMessage = "Visceral Fat Percentage must be between 0 and 100.")]
        [DisplayName("Visceral Fat Percentage")]
        public double VisceralFat { get; set; }


    }
}
