using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BLModels
{
    public class BLVideo
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int GenreId { get; set; }

        public int TotalSeconds { get; set; }

        public string? StreamingUrl { get; set; }

        public int? ImageId { get; set; }
    }
}
