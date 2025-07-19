using System;
using System.Collections.Generic;

namespace IMAP_ContentProcessor_BlazorWebApp.Infrastructure.Models;

public partial class OrderDetail
{
    public int Id { get; set; }

    public int? MailId { get; set; }

    public string? ProductName { get; set; }

    public int? ProductQuantity { get; set; }

    public decimal? Price { get; set; }

    public virtual Mail? Mail { get; set; }
}
