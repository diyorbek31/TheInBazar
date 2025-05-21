using Microsoft.EntityFrameworkCore;
using TheInBazar.Data.DbContexts;
using TheInBazar.Service.Mappers;
using TheInBazar.Service.Interfaces;
using TheInBazar.Service.Services;
using TheInBazar.Data.IRepositories;
using TheInBazar.Data.Repositories;
using TheInBazar.Api.Extensions;
using TheInBazar.Api.MiddleWares;
using Serilog;

namespace TheInBazar.Api
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add controllers
            builder.Services.AddControllers();

            // Add Swagger/OpenAPI
            // Assuming this is an extension method in your project

            // Check if the app is running in test mode
            var isTesting = builder.Environment.EnvironmentName == "InBazartesting";
            if (!isTesting)
            {
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            }

            // Register services and automapper
            builder.Services.AddCustomService();
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Configure Serilog
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);

            var app = builder.Build();

            // Configure middleware pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<ExceptionHandlerMiddleWare>();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }

    public partial class Program { }
}
