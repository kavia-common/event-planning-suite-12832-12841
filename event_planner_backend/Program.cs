using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;

var builder = WebApplication.CreateBuilder(args);

// Ocean Professional API metadata
var apiTitle = "Event Planner API";
var apiDescription = "Modern, secure REST API for planning and managing events with guest coordination and scheduling.\n\nOcean Professional: blue (#2563EB) base with amber (#F59E0B) accents.";
var apiVersion = "v1";

// Add services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Swagger/OpenAPI with Ocean Professional styling
builder.Services.AddOpenApiDocument(settings =>
{
    settings.Title = apiTitle;
    settings.Version = apiVersion;
    settings.Description = apiDescription;
    settings.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        Description = "Enter a valid JWT Bearer token to authorize. Example: Bearer eyJhbGciOi..."
    });
    settings.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
    settings.PostProcess = document =>
    {
        document.Info.Contact = new OpenApiContact
        {
            Name = "Event Planner Platform",
            Email = "support@example.com",
            Url = "https://example.com"
        };
        document.Info.License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = "https://opensource.org/licenses/MIT"
        };
        document.Tags = new List<OpenApiTag>
        {
            new OpenApiTag { Name = "Auth", Description = "User authentication and token issuance" },
            new OpenApiTag { Name = "Users", Description = "User profile and account operations" },
            new OpenApiTag { Name = "Events", Description = "Event lifecycle and scheduling" },
            new OpenApiTag { Name = "Guests", Description = "Guest invitations and RSVP" }
        };
    };
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowCredentials()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Authentication & JWT config (from env or appsettings)
var jwtIssuer = builder.Configuration["JWT__Issuer"] ?? "event-planner";
var jwtAudience = builder.Configuration["JWT__Audience"] ?? "event-planner-clients";
var jwtKey = builder.Configuration["JWT__Key"] ?? "super-secret-development-key-change-me";
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

builder.Services.AddAuthorization();

// Dependency Injection: repositories and services
builder.Services.AddSingleton<EventPlanner.Domain.Repositories.IUserRepository, EventPlanner.Infrastructure.Repositories.InMemoryUserRepository>();
builder.Services.AddSingleton<EventPlanner.Domain.Repositories.IEventRepository, EventPlanner.Infrastructure.Repositories.InMemoryEventRepository>();
builder.Services.AddSingleton<EventPlanner.Domain.Repositories.IGuestRepository, EventPlanner.Infrastructure.Repositories.InMemoryGuestRepository>();

builder.Services.AddScoped<EventPlanner.Application.Services.IAuthService, EventPlanner.Application.Services.AuthService>();

// Register application services with proper generic type parameters
builder.Services.AddScoped<EventPlanner.Application.Services.IUserService, EventPlanner.Application.Services.UserService>();
builder.Services.AddScoped<EventPlanner.Application.Services.IEventService, EventPlanner.Application.Services.EventService>();
builder.Services.AddScoped<EventPlanner.Application.Services.IGuestService, EventPlanner.Application.Services.GuestService>();

var app = builder.Build();

app.UseCors("AllowAll");

app.UseOpenApi();
app.UseSwaggerUi(config =>
{
    config.Path = "/docs";
    config.DocumentTitle = "Event Planner API Docs";
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => new
{
    message = "Healthy",
    theme = "Ocean Professional",
    palette = new { primary = "#2563EB", secondary = "#F59E0B", error = "#EF4444" }
});

app.Run();
