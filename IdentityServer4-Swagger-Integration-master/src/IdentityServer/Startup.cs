using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace IdentityServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc();

            services.AddIdentityServer()
                .AddTestUsers(TestUsers.Users)
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients())
                .AddDeveloperSigningCredential(false);
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            app.UseDeveloperExceptionPage();

            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}