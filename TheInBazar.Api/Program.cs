using Microsoft.EntityFrameworkCore;
using TheInBazar.Data.DbContexts;
using TheInBazar.Data.IRepositories;
using TheInBazar.Data.Repositories;
using TheInBazar.Service.Mappers;
using TheInBazar.Api.MiddleWares;
using TheInBazar.Service.Interfaces;
using TheInBazar.Service.Services;
using Serilog;
using TheInBazar.Api.Extensions;

namespace TheInBazar.Api
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddCustomService();
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Configure Serilog logging
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
}
