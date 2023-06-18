using BL.BLModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PublicModule.ViewModel
{
    public class VMCountries
    {
        [DisplayName("Username")]
        public string Username { get; set; }

        [DisplayName("FirstName")]
        public string FirstName { get; set; }

        [DisplayName("LastName")]
        public string LastName { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("Password2")]
        public string Password2 { get; set; }

        [DisplayName("Country")]
        public int CountryId { get; set; }
    }
}
