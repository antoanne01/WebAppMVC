namespace PublicModule.Models
{
    public class ChooseVideoContents
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int? ImageId { get; set; }
    }
}
