using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace GeekShopping.IdentityServer.Configuration
{
    public static class IdentityConfiguration
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";

        // Identity resource, nomes de grupos de claims que podem ser protegido
        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>()
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile()
            };

        // Identity Scope - Objectos com informações do perfil (Nome, Sobrenome, Username), Identity Claim
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>()
            {
                new ApiScope("geek_shopping", "GeekShopping Server"),
                new ApiScope(name: "read", "Read data."),
                new ApiScope(name: "write", "Write data."),
                new ApiScope(name: "delete", "Delete data.")
            };

        // Client - componente de software que requisita um recurso a um server
        public static IEnumerable<Client> Clients =>
            new List<Client>()
            {
                new Client // client generico
                {
                    ClientId = "client",
                    ClientSecrets =
                    {
                        new Secret("my_super_secret".Sha256())
                    },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes =
                    {
                        "read", "write", "profile"
                    }
                },
                new Client // client especifico do geek
                {
                    ClientId = "geek_shopping",
                    ClientSecrets =
                    {
                        new Secret("my_super_secret".Sha256())
                    },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = {"http://localhost:44396/signin-oidc"}, // porta ssl: 44396 ; porta normal = 46179
                    PostLogoutRedirectUris = { "http://localhost:44396/signout-callback-oidc" },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "geek_shopping"
                    }
                }
            };
    }
}
