using System.ComponentModel;

namespace AdminModule.ViewModel
{
    public class VMDeleteVideo
    {
        [DisplayName("Id")]
        public int Id { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; } = null!;
    }
}
