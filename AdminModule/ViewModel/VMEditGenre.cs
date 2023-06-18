using System.ComponentModel;

namespace AdminModule.ViewModel
{
    public class VMEditGenre
    {
        [DisplayName("Name")]
        public string Name { get; set; } = null!;

        [DisplayName("Description")]
        public string? Description { get; set; }
    }
}
