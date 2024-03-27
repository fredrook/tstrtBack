#region IMPORTA��ES
using API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence.Contextos;
using System;
#endregion

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<DataContext>();
                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                    {
                        context.Database.Migrate();
                    }
                    else
                    {
                        Console.WriteLine("A aplica��o est� rodando em um ambiente diferente de desenvolvimento. As migra��es n�o ser�o aplicadas automaticamente.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao aplicar migra��es: " + ex.Message);
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        })
        .ConfigureServices((hostContext, services) =>
        {
            var configuration = hostContext.Configuration;
            services.AddSingleton(configuration);
            services.AddDbContext<Context>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        });

    }
}