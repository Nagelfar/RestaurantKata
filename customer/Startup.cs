using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Customer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private void ConfigureApis(IServiceCollection services)
        {
            services.AddHttpClient();

            var apiConfiguration = Configuration.GetSection("APIs");

            services.AddHttpClient("GuestExperience")
                .ConfigureHttpClient(client =>
                    client.BaseAddress = apiConfiguration.GetValue<Uri>("GuestExperience")
                );
            services.AddHttpClient("TableService")
                .ConfigureHttpClient(client =>
                    client.BaseAddress = apiConfiguration.GetValue<Uri>("TableService")
                );
            services.AddHttpClient("Billing")
                .ConfigureHttpClient(client =>
                    client.BaseAddress = apiConfiguration.GetValue<Uri>("Billing")
                );
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddControllers();

            services.AddSingleton(new EventStore());

            ConfigureApis(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
