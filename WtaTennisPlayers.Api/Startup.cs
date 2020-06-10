using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WtaTennisPlayers.Services.Implementations;
using WtaTennisPlayers.Services.Interfaces;

namespace WtaTennisPlayers.Api
{
    /// <summary>
    /// Class responsible for any startup configuration.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">An implementation of an <see cref="IServiceCollection"/> for configuring application configuration.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => options.EnableEndpointRouting = false);

            // Add OData and Memory Cache services
            services.AddOData();
            services.AddMemoryCache();

            // Add custom service implementations
            services.AddScoped<IPlayerDataService, PlayerDataService>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">An <see cref="IApplicationBuilder"/> to configure the application's request pipeline.</param>
        /// <param name="env">An <see cref="IWebHostEnvironment"/> for surfacing information about the environment the application is running on.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseMvc(routeBuilder =>
            {
                // Additional step required for OData to work - enable DI and setup the OData functionality that clients can call (Select, Count, etc.)
                routeBuilder.EnableDependencyInjection();
                routeBuilder.Expand().Select().Count().OrderBy().Filter();
            });
        }
    }
}
