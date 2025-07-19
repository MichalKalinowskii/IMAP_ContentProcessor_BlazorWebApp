using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMAP_ContentProcessor_BlazorWebApp.Domain
{
    public static class Registration
    {
        public static void AddDomain(this IServiceCollection services)
        {
            services.AddScoped<ILoginHandler, LoginHandler>();
        }
    }
}
