using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models 
{
    public class RegisterUser : BaseEntity
    {
        public int RegisterUserId { get; set; }
        [Required(ErrorMessage = "Your name is required.")]
        [MinLengthAttribute(2, ErrorMessage = "Your name must be at least {1} characters long.")]
        [RegularExpression(@"^[a-zA-Z ,.'-]+$", ErrorMessage = "Your name cannot contain numbers or most symbols.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Your email address is required.")]
        [EmailAddressAttribute(ErrorMessage = "You need to enter a valid email address.")]
        public string EmailAddress {get; set; }

        [Required(ErrorMessage = "A password is required.")]
        [MinLengthAttribute(8, ErrorMessage = "Your password must be at least {1} characters long.")]
        public string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Your passwords do not match.")]
        public string ConfirmPassword {get; set; }

        [Required(ErrorMessage = "A description is required.")]
        [MinLengthAttribute(5, ErrorMessage = "Your description must be at least {1} characters long.")]
        public string Description { get; set; }

    }

}