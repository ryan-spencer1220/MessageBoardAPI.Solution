using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using MessageBoard.Models;

using Microsoft.AspNetCore.Mvc;


namespace MessageBoard
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
          // Enable CORS policy for specific websites/servers
            services.AddCors(options =>
            {
                options.AddPolicy("Policy",
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:5001", "http://localhost:5000");
                    });
            });
                services.AddDbContext<MessageBoardContext>(opt =>
                    opt.UseMySql(Configuration["ConnectionStrings:DefaultConnection"], ServerVersion.AutoDetect(Configuration["ConnectionStrings:DefaultConnection"])));
                services.AddControllers();
                // Add API Versioning
                services.AddApiVersioning(o => {
                    o.ReportApiVersions = true;
                    o.AssumeDefaultVersionWhenUnspecified = true;
                    o.DefaultApiVersion = new ApiVersion(1, 0);
                    });
                services.AddSwaggerGen();
            }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            // JWT Config
            app.UseAuthentication();

            app.UseRouting();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            }
            );

            app.UseAuthorization();

          // Enable CORS policy for specific websites/servers
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
