using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EmailServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Configure/Add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomDatabase(Configuration);
            services.AddCustomAuth(Configuration);
            services.AddCustomControllers();
            services.AddCustomBehaviors();
            services.AddCustomTransientServices(Configuration);
        }

        // This method gets called by the runtime. Configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            // For CORS, the browser makes a request option first,
            // shorting response so request error is properly handled
            app.UseExpressPreflight();

            // Setup env methods
            if (env.IsDevelopment())
            {
                app.UseCustomExceptionHandler();
                // app.UseDeveloperExceptionPage();
            }
            else
            {
                // Customize exception handler
                app.UseCustomExceptionHandler();
                app.UseHsts();
            }

            // NOTES: Move this in the env check later 
            // if you still deem it necessary. Currently disabled Postman SSL verifiation
            app.UseHttpsRedirection();

            // Load index.html for root path (Custom UseStaticFiles)
            app.UseCustomHTMLPage();

            app.UseRouting();

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                // .WithOrigins("https://ctsemail.com")
                .AllowAnyMethod()
                .AllowAnyHeader()
                // .AllowCredentials()
                );

            app.UseCustomPrivacy();

            // Customize status code error responses
            app.UserCustomStatusCodePage();

            // Setup authentication services
            // app.UseAuthorization();
            // app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Initialize the mock data (Only do it once!)
            MockData.Initialize(app);
        }
    }
}
