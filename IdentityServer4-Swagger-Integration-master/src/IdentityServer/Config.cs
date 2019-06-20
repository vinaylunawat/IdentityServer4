using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        //public static IEnumerable<IdentityResource> GetIdentityResources()
        //{
        //    return new IdentityResource[]
        //    {
        //        new IdentityResources.OpenId(),
        //        new IdentityResources.Profile(),
        //    };
        //}

        //public static IEnumerable<ApiResource> GetApis()
        //{
        //    return new[]
        //    {
        //        new ApiResource("demo_api", "Demo API with Swagger")
        //    };
        //}

        //public static IEnumerable<Client> GetClients()
        //{
        //    return new[]
        //    {
        //        new Client
        //        {
        //            ClientId = "demo_api_swagger",
        //            ClientName = "Swagger UI for demo_api",
        //            AllowedGrantTypes = GrantTypes.Code,
        //            AllowAccessTokensViaBrowser = true,
        //            RedirectUris =
        //            {
        //                "http://localhost:5001/oauth2-redirect.html",
        //                "http://localhost:5001/o2c.html"
        //            },
        //            AllowedScopes = { "openid", "profile", "demo_api" },
        //            RequireClientSecret = false,
        //            RequirePkce = true,
        //            RequireConsent = false,
        //            AllowedCorsOrigins =
        //            {
        //                "http://localhost:5001",
        //                "http://localhost:5000"
        //            },
        //            ClientSecrets = {new Secret("acf2ec6fb01a4b698ba240c2b10a0243".Sha256())},
        //        }
        //    };
        //}

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource("demo_api", "My API #1")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                new Client
                {
                    ClientId = "demo_api_swagger",
                    ClientName = "Swagger UI for demo_api",
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris =
                    {
                        "http://localhost:5001/oauth2-redirect.html",
                        "http://localhost:5001/o2c.html"
                    },
                    AllowedScopes = { "openid", "profile", "demo_api" },
                    RequireClientSecret = true,
                    RequirePkce = true,
                    RequireConsent = false,
                    AllowedCorsOrigins =
                    {
                        "http://localhost:5001",
                        "http://localhost:5000"
                    },
                    ClientSecrets = {new Secret("acf2ec6fb01a4b698ba240c2b10a0243".Sha256())},
                    AllowOfflineAccess = true,
                    AllowPlainTextPkce = false
                }
            };
        }
    }
}