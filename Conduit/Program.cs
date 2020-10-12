using Blazored.LocalStorage;
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
            builder.Services.AddScoped<AuthService, AuthService>();
            builder.Services.AddScoped<ConduitAuthorizationHandler, ConduitAuthorizationHandler>();


            builder.Services.AddHttpClient<ConduitClient>(client => client.BaseAddress = new Uri("https://conduit.productionready.io/"))
                            .AddHttpMessageHandler<ConduitAuthorizationHandler>();
            builder.Services.AddHttpClient<PublicClient>(client => client.BaseAddress = new Uri("https://conduit.productionready.io/"));

            //builder.Services.AddScoped<OrderState>();

            await builder.Build().RunAsync();
        }
    }
}
