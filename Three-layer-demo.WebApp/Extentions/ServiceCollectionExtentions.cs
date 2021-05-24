using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Three_layer_demo.Domain.Context;
using Three_layer_demo.Domain.Interfaces;
using Three_layer_demo.Domain.Interfaces.Services;
using Three_layer_demo.Domain.Models;
using Three_layer_demo.Infrastructure;
using Three_layer_demo.Service;

namespace Three_layer_demo.WebApp.Extentions
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            services.AddDbContext<DemoContext>(options =>
            {
                options.UseSqlServer(AppSettings.ConnectionString, sqlOptions => sqlOptions.CommandTimeout(120));
                options.UseLazyLoadingProxies();
            });

            services.AddScoped<Func<DemoContext>>((provider) => () => provider.GetService<DemoContext>());
            services.AddScoped<DbFactory>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IActorService, ActorService>();

            return services;
        }
    }
}
