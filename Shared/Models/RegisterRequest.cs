using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text;

namespace CUnity.Shared.Models
{
    public class RegisterRequest 
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match!")]
        public string PasswordConfirm { get; set; }
        [Required]
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string RoleName { get; set; }
        public List<string> Roles { get; set; }
        public int ClientId { get; set; }

        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (!string.IsNullOrEmpty(ClientEmail))
        //    {
        //        MailAddress address = new MailAddress(ClientEmail);
        //        string clientdomain = address.Host; // host contains yahoo.com if string is xyz@yahoo.com
        //        address = new MailAddress(Email);
        //        string userdomain = address.Host; // host contains yahoo.com
        //        address = null;
        //        if (clientdomain != userdomain)
        //        {
        //            yield return new ValidationResult(
        //               errorMessage: "User Email address Domain is not matching with Client Domain",
        //               memberNames: new[] { "Email" }
        //          );
        //        }
        //    }
        //}
    }
}
