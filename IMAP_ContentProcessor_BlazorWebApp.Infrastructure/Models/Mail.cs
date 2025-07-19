using System;
using System.Collections.Generic;

namespace IMAP_ContentProcessor_BlazorWebApp.Infrastructure.Models;

public partial class Mail
{
    public int Id { get; set; }

    public int? TokenId { get; set; }

    public string? Content { get; set; }

    public byte[]? Eml { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Token? Token { get; set; }
}
