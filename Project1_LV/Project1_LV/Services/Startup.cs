using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Project1_LV.Services;  // Ensure your services are correctly referenced

namespace Project1_LV
{
    public class Startup
    {
        // Configure services in this method
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Register KeyManagementService as a singleton
            services.AddSingleton<KeyManagementService>();
        }

        // Configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}