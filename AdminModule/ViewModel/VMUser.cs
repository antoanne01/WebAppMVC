using BL.BLModels;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdminModule.ViewModel
{
    public class VMUser
    {
        [DisplayName("Id")]
        [ValidateNever]
        public int Id { get; set; }

        [DisplayName("CreatedAt")]
        [ValidateNever]
        public DateTime CreatedAt { get; set; }

        [DisplayName("DeletedAt")]
        [ValidateNever]
        public DateTime? DeletedAt { get; set; }

        [DisplayName("Username")]
        public string Username { get; set; } = null!;

        [DisplayName("First name")]
        public string FirstName { get; set; } = null!;

        [DisplayName("Last name")]
        public string LastName { get; set; } = null!;

        [DisplayName("E-mail")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [ValidateNever]
        public string PwdHash { get; set; } = null!;

        [ValidateNever]
        public string PwdSalt { get; set; } = null!;

        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("Repeat password")]
        [Compare("Password")]
        public string Password2 { get; set; }

        public string? Phone { get; set; }

        [ValidateNever]
        public bool IsConfirmed { get; set; }

        [ValidateNever]
        public string? SecurityToken { get; set; }

        [ValidateNever]
        [DisplayName("Country")]
        public int CountryOfResidenceId { get; set; }

        [ValidateNever]
        [DisplayName("Country")]
        public string CountryName { get; set; }

        [DisplayName("Country")]
        public int CountryId { get; set; }

        [ValidateNever]
        public IEnumerable<BLCountry> Countries { get; set; }
    }
}
