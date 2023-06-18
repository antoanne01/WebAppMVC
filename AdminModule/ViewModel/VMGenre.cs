using System.ComponentModel;

namespace AdminModule.ViewModel
{
    public class VMGenre
    {
        [DisplayName("Id")]
        public int Id { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; } = null!;

        [DisplayName("Description")]
        public string? Description { get; set; }
    }
}
