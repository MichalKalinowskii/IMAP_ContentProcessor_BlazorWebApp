using System;
using System.Collections.Generic;

namespace IMAP_ContentProcessor_BlazorWebApp.Infrastructure.Models;

public partial class Token
{
    public int Id { get; set; }

    public int? EmailGeneratedId { get; set; }

    public string? Email { get; set; }

    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }

    public virtual ICollection<Mail> Mail { get; set; } = new List<Mail>();
}
