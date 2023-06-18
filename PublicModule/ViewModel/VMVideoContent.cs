using System.ComponentModel;

namespace PublicModule.ViewModel
{
    public class VMVideoContent
    {
        [DisplayName("Id")]
        public int Id { get; set; }

        [DisplayName("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; } = null!;

        [DisplayName("Description")]
        public string? Description { get; set; }

        [DisplayName("Genre")]
        public int GenreId { get; set; }

        [DisplayName("TotalSeconds")]
        public int TotalSeconds { get; set; }

        [DisplayName("StreamingUrl")]
        public string? StreamingUrl { get; set; }

        [DisplayName("Image")]
        public int? ImageId { get; set; }
    }
}
