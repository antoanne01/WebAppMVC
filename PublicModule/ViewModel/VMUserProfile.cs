using System.ComponentModel;

namespace PublicModule.ViewModel
{
    public class VMUserProfile
    {
        [DisplayName("Id")]
        public int Id { get; set; }

        [DisplayName("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [DisplayName("Username")]
        public string Username { get; set; } = null!;

        [DisplayName("FirstName")]
        public string FirstName { get; set; } = null!;

        [DisplayName("LastName")]
        public string LastName { get; set; } = null!;

        [DisplayName("Email")]
        public string Email { get; set; } = null!;

        [DisplayName("Phone")]
        public string? Phone { get; set; }

        [DisplayName("IsConfirmed")]
        public bool IsConfirmed { get; set; }

        [DisplayName("CountryOfResidenceId")]
        public int CountryOfResidenceId { get; set; }
    }
}
