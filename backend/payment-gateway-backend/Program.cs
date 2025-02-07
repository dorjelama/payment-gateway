using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using payment_gateway_backend.Configurations;
using payment_gateway_backend.Data;
using payment_gateway_backend.Helpers;
using payment_gateway_backend.Repositories;
using payment_gateway_backend.Repositories.Implementations;
using payment_gateway_backend.Repositories.Interfaces;
using payment_gateway_backend.Services;
using payment_gateway_backend.Services.Implementations;
using payment_gateway_backend.Services.Interfaces;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;

namespace payment_gateway_backend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((context, services, configuration) =>
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
        );
        builder.Services.AddDbContext<PaymentGatewayDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        );
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(
                option =>
                {
                    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Clover Tech Balance Book", Version = "V1" });
                    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please enter a valid token",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        Scheme = "Bearer"
                    });
                    option.AddSecurityRequirement(new OpenApiSecurityRequirement
                       {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type=ReferenceType.SecurityScheme,
                                        Id="Bearer"
                                    }
                                },
                                Array.Empty<string>()
                            }
                       });
                }
               );
        builder.Services.AddAutoMapper(typeof(MappingProfile));

        builder.Services.AddSingleton<JwtHelper>();
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

        builder.Services.AddAuthorization();

        builder.Services.AddSingleton<Publisher>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            return new Publisher(config);
        });

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddSingleton<InMemoryPaymentTransactionRepository>();
        builder.Services.AddSingleton<InMemoryEventLogRepository>();
        builder.Services.AddSingleton<IPaymentSimulator, PaymentSimulatorService>();
        builder.Services.AddHostedService<Consumer>();
        builder.Services.AddScoped<EventLogService>();

        var app = builder.Build();

        DbInitializer.Seed(app.Services);

        app.UseSerilogRequestLogging();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.DefaultModelsExpandDepth(-1);
            c.DefaultModelExpandDepth(-1);
            c.DocExpansion(DocExpansion.None);
            c.EnableFilter();
        });

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
