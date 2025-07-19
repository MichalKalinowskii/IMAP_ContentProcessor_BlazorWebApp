using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMAP_ContentProcessor_BlazorWebApp.Domain
{
    public interface ILoginHandler
    {
        Task GetOrdersInformation(AuthenticateResult authenticateResult);
    }
}
    