using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using payment_gateway_backend.Configurations;
using payment_gateway_backend.Data;
using payment_gateway_backend.Helpers;
using payment_gateway_backend.Repositories.Implementations;
using payment_gateway_backend.Repositories.Interfaces;
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
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
        });
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

        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
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

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
        builder.Services.AddScoped<IEventLogRepository, EventLogRepository>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddSingleton<IPaymentSimulator, PaymentSimulatorService>();
        builder.Services.AddScoped<IPaymentProcessService, PaymentProcessService>();
        builder.Services.AddScoped<IPaymentTransactionService, PaymentTransactionService>();
        builder.Services.AddScoped<JwtHelper>();
        builder.Services.AddScoped<IEventLogService, EventLogService>();
        builder.Services.AddHostedService<Consumer>();
        builder.Services.AddSingleton<INotificationService, EmailNotificationService>();
        builder.Services.AddHostedService<FailureConsumer>();

        var app = builder.Build();

        DbInitializer.Seed(app.Services);

        app.UseCors("AllowAll");
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
