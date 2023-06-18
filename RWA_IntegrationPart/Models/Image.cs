﻿using System;
using System.Collections.Generic;

namespace RWA_IntegrationPart.Models;

public partial class Image
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
}
