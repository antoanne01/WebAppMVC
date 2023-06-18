using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using BL.BLModels;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PublicModule.ViewModel
{
    public class VMRegister
    {
        [DisplayName("User name")]
        public string Username { get; set; }

        [DisplayName("First name")]
        public string FirstName { get; set; }

        [DisplayName("Last name")]
        public string LastName { get; set; }

        [DisplayName("E-mail")]
        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("Repeat password")]
        [Compare("Password")]
        public string Password2 { get; set; }

        [DisplayName("Phone")]
        public string? Phone { get; set; }

        [DisplayName("Country")]
        public int CountryId { get; set; }

        [ValidateNever]
        public IEnumerable<BLCountry> Countries { get; set; }
    }
}
