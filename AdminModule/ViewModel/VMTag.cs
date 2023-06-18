using System.ComponentModel;

namespace AdminModule.ViewModel
{
    public class VMTag
    {
        [DisplayName("Id")]
        public int Id { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; } = null!;
    }
}
