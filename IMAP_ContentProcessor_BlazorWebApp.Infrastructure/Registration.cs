using IMAP_ContentProcessor_BlazorWebApp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMAP_ContentProcessor_BlazorWebApp.Infrastructure
{
    public static class Registration
    {
        public static void AddInfrastucture(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MySqlConnectionString");

            services.AddDbContext<ZamowieniaImapContext>(options => 
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            );

            services.AddSingleton(x => new SqlConnectionFactory(connectionString!));                  
        }
    }
}
