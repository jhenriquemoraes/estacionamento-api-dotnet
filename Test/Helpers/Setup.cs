using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using MinimalAPI.Dominio.Interfaces;
using MinimalAPI.Dominio.Servicos;
using Test.Mocks;

namespace Test.Helpers
{
    public class Setup
    {
        public const string PORT = "5001";
        public static string? Token { get; set; }

        public static TestContext testContext = default!;
        public static WebApplicationFactory<Startup> http = default!;

        public static HttpClient client = default!;

        public static void ClassInit(TestContext testContext)
        {
            Setup.testContext = testContext;

            Setup.http = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
            {
                builder.UseSetting("http_port", Setup.PORT).UseEnvironment("Testing");

                builder.ConfigureServices(services =>
                {
                    services.AddScoped<IAdministradorServicos, AdministradorServicoMock>();

                });
            });

            Setup.client = Setup.http.CreateClient();

        }
            public static void ClassCleanup() => Setup.http.Dispose(); 
    }
}