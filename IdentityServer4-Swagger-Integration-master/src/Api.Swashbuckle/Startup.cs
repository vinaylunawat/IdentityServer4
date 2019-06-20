using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Swashbuckle
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "cookie";
                options.DefaultSignInScheme = "cookie";
                options.DefaultChallengeScheme = "oidc";
            }).AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost:5000"; // auth server base endpoint (will use to search for disco doc)
                    options.ApiName = "demo_api"; // required audience of access tokens
                    options.RequireHttpsMetadata = false; // dev only!
                })
               .AddCookie("cookie")
               .AddOpenIdConnect("oidc", options =>
               {
                   options.Authority = "http://localhost:5000";
                   options.RequireHttpsMetadata = false; // dev only
                   options.ClientId = "demo_api_swagger";
                   options.ClientSecret = "acf2ec6fb01a4b698ba240c2b10a0243";
                   options.ResponseType = "code";
                   options.ResponseMode = "form_post";
                   options.CallbackPath = "/oauth2-redirect.html";
                   options.Events.OnRedirectToIdentityProvider = context =>
                   {
                       // only modify requests to the authorization endpoint
                       if (context.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication)
                       {
                           // generate code_verifier
                           var codeVerifier = CryptoRandom.CreateUniqueId(32);

                           // store codeVerifier for later use
                           context.Properties.Items.Add("code_verifier", codeVerifier);

                           // create code_challenge
                           string codeChallenge;
                           using (var sha256 = SHA256.Create())
                           {
                               var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                               codeChallenge = Base64Url.Encode(challengeBytes);
                           }

                           // add code_challenge and code_challenge_method to request
                           context.ProtocolMessage.Parameters.Add("code_challenge", codeChallenge);
                           context.ProtocolMessage.Parameters.Add("code_challenge_method", "S256");
                       }

                       return Task.CompletedTask;
                   };

                   options.Events.OnAuthorizationCodeReceived = context =>
                   {
                       // only when authorization code is being swapped for tokens
                       if (context.TokenEndpointRequest?.GrantType == OpenIdConnectGrantTypes.AuthorizationCode)
                       {
                           // get stored code_verifier
                           if (context.Properties.Items.TryGetValue("code_verifier", out var codeVerifier))
                           {
                               // add code_verifier to token request
                               context.TokenEndpointRequest.Parameters.Add("code_verifier", codeVerifier);
                           }
                       }

                       return Task.CompletedTask;
                   };
               });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "Protected API", Version = "v1" });

                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Flow = "authorizationCode", // just get token via browser (suitable for swagger SPA)
                    AuthorizationUrl = "http://localhost:5000/connect/authorize",
                    TokenUrl = "http://localhost:5000/connect/token",
                    Scopes = new Dictionary<string, string> { { "demo_api", "Demo API - full access" } },
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>(); // Required to use access token
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseAuthentication();

            // Swagger JSON Doc
            app.UseSwagger();

            // Swagger UI
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                options.RoutePrefix = string.Empty;
                options.OAuthClientId("demo_api_swagger");
                options.OAuthAppName("Demo API - Swagger"); // presentation purposes only
            });

            app.UseMvc();
        }
    }

    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            //var hasAuthorize = context.ControllerActionDescriptor.GetControllerAndActionAttributes(true).OfType<AuthorizeAttribute>().Any();

            if (true)
            {
                operation.Responses.Add("401", new Response { Description = "Unauthorized" });
                operation.Responses.Add("403", new Response { Description = "Forbidden" });

                operation.Security = new List<IDictionary<string, IEnumerable<string>>>
                {
                    new Dictionary<string, IEnumerable<string>> {{ "oAuth2AuthCode", new[] {"demo_api"}}, { "oauth2", new[] { "demo_api" } } }
                };
            }
        }
    }
}
