using System.IO;
using Asda.Integration.Business.Services;
using Asda.Integration.Business.Services.Adapters;
using Asda.Integration.Business.Services.Config;
using Asda.Integration.Service.Intefaces;
using Asda.Integration.Service.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;

namespace SampleChannel
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddControllers()
                .AddNewtonsoftJson(opt => { opt.SerializerSettings.Converters.Add(new StringEnumConverter()); });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "SampleChannel", Version = "v1"});
            });
            services.AddSwaggerGenNewtonsoftSupport();

            services.AddScoped<IOrderService, OrderService>();

            services.AddScoped<IProductService, ProductService>();

            services.AddScoped<IXmlService, XmlService>();

            services.AddScoped<IFtpService, FtpService>();

            services.AddScoped<IFtpConfigManagerService, FtpConfigManagerService>();

            services.AddScoped<ILocalConfigManagerService, LocalConfigManagerService>();

            services.AddScoped<IRemoteConfigManagerService, RemoteConfigManagerService>();

            services.AddSingleton<IConfigStages, ConfigStages>();

            services.AddSingleton<IUserConfigAdapter, UserConfigAdapter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SampleChannel v1"));
            }

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images")),
                RequestPath = new PathString("/logo")
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}