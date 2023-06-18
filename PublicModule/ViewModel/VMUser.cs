using System.ComponentModel;

namespace PublicModule.ViewModel
{
    public class VMUser
    {
        [DisplayName("Id")]
        public int Id { get; set; }

        [DisplayName("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [DisplayName("DeletedAt")]
        public DateTime? DeletedAt { get; set; }

        [DisplayName("Username")]
        public string Username { get; set; } = null!;

        [DisplayName("Firstname")]
        public string FirstName { get; set; } = null!;

        [DisplayName("Lastname")]
        public string LastName { get; set; } = null!;

        [DisplayName("Email")]
        public string Email { get; set; } = null!;

        public string PwdHash { get; set; } = null!;

        public string PwdSalt { get; set; } = null!;

        [DisplayName("Phone")]
        public string? Phone { get; set; }

        [DisplayName("IsConfirmed")]
        public bool IsConfirmed { get; set; }

        public string? SecurityToken { get; set; }

        [DisplayName("CountryOfResidence")]
        public int CountryOfResidenceId { get; set; }
    }
}
