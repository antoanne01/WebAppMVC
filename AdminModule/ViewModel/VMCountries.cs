using System.ComponentModel;

namespace AdminModule.ViewModel
{
    public class VMCountries
    {
        [DisplayName("Id")]
        public int Id { get; set; }

        [DisplayName("Code")]
        public string Code { get; set; } = null!;

        [DisplayName("Name")]
        public string Name { get; set; } = null!;
    }
}
