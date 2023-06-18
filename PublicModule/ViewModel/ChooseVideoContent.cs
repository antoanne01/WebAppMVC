using System.ComponentModel;

namespace PublicModule.ViewModel
{
    public class ChooseVideoContent
    {
        [DisplayName("Id")]
        public int Id { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; } = null!;

        [DisplayName("Description")]
        public string? Description { get; set; }

        [DisplayName("Image")]
        public int? ImageId { get; set; }
    }
}
