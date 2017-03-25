using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models
{
    public class LoginUser : BaseEntity
    {
        public int LoginUserId { get; set; }

        [Required(ErrorMessage = "Your email address is required.")]
        [EmailAddressAttribute(ErrorMessage = "You need to enter a valid email address.")]
        public string EmailAddress {get; set; }

        [Required(ErrorMessage = "A password is required.")]
        [MinLengthAttribute(8, ErrorMessage = "Your password must be at least {1} characters long.")]
        public string Password { get; set; }
    }

}