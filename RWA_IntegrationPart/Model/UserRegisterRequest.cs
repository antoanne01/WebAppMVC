using System.ComponentModel.DataAnnotations;

namespace RWA_IntegrationPart.Model
{
    public class UserRegisterRequest
    {
        [Required, StringLength(50, MinimumLength = 6)]
        public string Username { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, StringLength(50, MinimumLength = 6)]
        public string Password { get; set; }

        [Required, StringLength(50, MinimumLength = 6)]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Email { get; set; }

        public string Phone { get; set; }

        [Required]
        public int CountryOfResidenceId { get; set; }
    }
}
