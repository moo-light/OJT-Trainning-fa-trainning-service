using Application.Commons;
using Application.Interfaces;
using Application.Utils;
using Hangfire;
using Hangfire.Logging;
using Infrastructures;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text.Json.Serialization;
using WebAPI;
using WebAPI.Middlewares;

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);

    // parse the configuration in appsettings
    var configuration = builder.Configuration.Get<AppConfiguration>();
    builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());
    // Them CORS cho tat ca moi nguoi deu xai duoc apis
    builder.Services.AddCors(options
        => options.AddDefaultPolicy(policy
            => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

    builder.Services.AddInfrastructuresService(configuration!.DatabaseConnection);
    builder.Services.AddWebAPIService(configuration!.JWTSecretKey);

    /*
        register with singleton life time
        now we can use dependency injection for AppConfiguration
    */
    builder.Services.AddSingleton(configuration);
    builder.Services.AddSwaggerGen(opt =>
    {
        opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend API", Version = "v1" });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        opt.IncludeXmlComments(xmlPath);

        opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });

        opt.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                new string[]{}
            }
        });
    });

    // Dang ki hangfire de thuc hien cron job
    builder.Services.AddHangfire(config => config
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseInMemoryStorage());
    builder.Services.AddHangfireServer();

    var app = builder.Build();
    // Modify log message of serilog 
    app.UseSerilogRequestLogging(configure =>
    {
        configure.MessageTemplate = "HTTP {RequestMethod} {RequestPath} ({UserId}) responded {StatusCode} in {Elapsed:0.0000}ms";
    });
    // Bat Cors
    app.UseCors();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend API"));
    }
    if (app.Environment.IsProduction())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend API"));
    }

    // add custom middlewares



    app.UseMiddleware<GlobalExceptionMiddlewareV2>();

    app.UseMiddleware<PerformanceMiddleware>();
    // App health check at root/healthchecks 
    app.MapHealthChecks("/healthchecks");

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // hangfire host dashboard at "/dashboard"
    app.MapHangfireDashboard("/dashboard");

    // call hangfire
    await app.StartAsync();
    RecurringJob.AddOrUpdate<ApplicationCronJob>(util => util.CheckAttendancesEveryDay(), 
        "0 22 * * *", TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
    RecurringJob.AddOrUpdate<IAssignmentService>(a => a.CheckOverDue(), 
        "* * * * *", TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
    RecurringJob.AddOrUpdate<ApplicationCronJob>(a => a.ExtractGradingDataEveryDay(),
        "0 23 * * *", TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
    RecurringJob.AddOrUpdate<ApplicationCronJob>(a => a.CheckFilesEveryday(),
        "0 0 * * *", TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
    await app.WaitForShutdownAsync();



}
catch (Exception ex)
{
    Log.Fatal(ex, "Host Terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
