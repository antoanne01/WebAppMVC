using System.ComponentModel;

namespace PublicModule.ViewModel
{
    public class VMLogin
    {
        [DisplayName("Username")]
        public string Username { get; set; }

        [DisplayName("Password")]
        public string Password { get; set; }

        public string? RedirectUrl { get; set; }
    }
}
