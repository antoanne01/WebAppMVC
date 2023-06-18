using System.ComponentModel.DataAnnotations;

namespace RWA_IntegrationPart.Model
{
    public class NotificationResponse
    {
        public int Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public string ReceiverEmail { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }
    }
}
