using Blazored.LocalStorage;
using Conduit.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Conduit
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddOptions();
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, ConduitAuthenticationStateProvider>();
            builder.Services.AddScoped<ConduitAuthorizationHandler, ConduitAuthorizationHandler>();

            builder.Services.AddHttpClient<ConduitService>(client => client.BaseAddress = new Uri("https://conduit.productionready.io/"))
                            .AddHttpMessageHandler<ConduitAuthorizationHandler>();
            builder.Services.AddHttpClient<AuthService>(client => client.BaseAddress = new Uri("https://conduit.productionready.io/"));

            await builder.Build().RunAsync();
        }
    }
}
