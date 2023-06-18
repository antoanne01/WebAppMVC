namespace PublicModule.Models
{
    public class UserProfiles
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Username { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public bool IsConfirmed { get; set; }

        public int CountryOfResidenceId { get; set; }
    }
}
