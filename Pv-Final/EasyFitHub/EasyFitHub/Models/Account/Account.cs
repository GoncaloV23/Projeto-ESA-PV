using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFitHub.Models.Account
{
    public class Account
    {
        public Account() { }
        public Account(AccountType accountType) { AccountType = accountType; }
        [Key]
        public int AccountId { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "User Name must be between 4 and 20 characters.")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Password must be bigger then 4 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        [NotMapped]
        public string ConfirmPassword { get; set; }

        public string? Token { get; set; }
        public string? RecoverCode { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }

        public AccountType AccountType { get; set; } = AccountType.UNDEFINED;
    }
}