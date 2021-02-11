using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebAssemblyOidc
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped<CustomAuthenticationMessageHandler>();
            builder.Services.AddHttpClient("api", opt => opt.BaseAddress = new Uri("https://demo.identityserver.io"))
                .AddHttpMessageHandler<CustomAuthenticationMessageHandler>();
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("api"));

            builder.Services.AddOidcAuthentication(opt =>
            {
                opt.ProviderOptions.Authority = "https://demo.identityserver.io";
                opt.ProviderOptions.ClientId = "interactive.public";
                opt.ProviderOptions.ResponseType = "code";
                opt.ProviderOptions.DefaultScopes.Add("api");
                opt.ProviderOptions.DefaultScopes.Add("email");
                opt.ProviderOptions.DefaultScopes.Add("profile");
            });

            await builder.Build().RunAsync();
        }
    }
    public class CustomAuthenticationMessageHandler : AuthorizationMessageHandler
    {
        public CustomAuthenticationMessageHandler(IAccessTokenProvider provder, NavigationManager nav) : base(provder, nav)
        {
            ConfigureHandler(new string[] { "https://demo.identityserver.io"});
        }
    }
}
