
using payment_gateway_backend.Configurations;
using Serilog;

namespace payment_gateway_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Host.UseSerilog((context, services, configuration) =>
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
            );

            builder.Services.AddAutoMapper(typeof(MappingProfile));

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseSerilogRequestLogging();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
