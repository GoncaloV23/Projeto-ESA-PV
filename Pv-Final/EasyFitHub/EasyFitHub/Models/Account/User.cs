using System.ComponentModel.DataAnnotations;

namespace EasyFitHub.Models.Account
{
    public class User : Account
    {
        public User() : base(AccountType.USER) { }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 50 characters.")]
        public string Name { get; set; } = "Não especificado";

        [Required(ErrorMessage = "Surname is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Surname must be between 1 and 50 characters.")]
        public string Surname { get; set; } = "Não especificado";

        [Required(ErrorMessage = "BirthDate is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateOnly), "1/1/1900", "1/1/2100", ErrorMessage = "BirthDate must be between 1900 and 2100.")]
        public DateOnly BirthDate { get; set; } = DateOnly.MaxValue;
    }
}
