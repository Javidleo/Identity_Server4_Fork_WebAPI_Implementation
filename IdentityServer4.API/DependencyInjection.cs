using Application.ServiceContract;
using Application.Services.Account;
using IdentityServer4.API.Common;

namespace IdentityServer4.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCustomIdentityServer(this IServiceCollection services)
        {
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddTestUsers(TestUsers.Users);

            return services;
        }

        
    }
}
