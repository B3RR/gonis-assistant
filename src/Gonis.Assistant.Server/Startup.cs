using Gonis.Assistant.Core.Middlewares;
using Gonis.Assistant.Server.Extensions;
using Gonis.Assistant.Server.HealthCheckers;
using Gonis.Assistant.Telegram;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Gonis.Assistant.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddLogger(Configuration);
            services.AddSettings(Configuration);
            services.AddTelegramBot();
            services.AddHttpClientForWebApi(Configuration);
            services.AddCustomHealthChecks();
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
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gonis Assistant v1"));

            app.UseMiddleware<HttpStatusCodeExceptionMiddleware>();

            // app.UseMiddleware<OptionsVerbMiddleware>();
            app.UseMiddleware<LoggerWebApiRequestMiddleware>();

            app.UseStaticFiles();

            app.UseRouting();
            app.UseCustomHealthCheckEndpoints();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
