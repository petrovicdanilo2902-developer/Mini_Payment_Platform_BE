using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PaymentPlatform.Application.Interfaces;
using PaymentPlatform.Domain.Entities;
using PaymentPlatform.Infrastructure.Data;
using PaymentPlatform.Infrastructure.Repositories;
using PaymentPlatform.Infrastructure.Services;
using PaymentPlatform.Application.Payments;
using PaymentPlatform.Infrastructure.PaymentProviders;

var builder = WebApplication.CreateBuilder(args);

// ✅ 1. Configuration setup (redom: appsettings → appsettings.{env} → User Secrets → Environment)
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddUserSecrets<Program>(optional: true) // <--- ovde se učitavaju tvoji local user secrets (Stripe, JWT itd.)
    .AddEnvironmentVariables();

var configuration = builder.Configuration;

// ✅ 2. DbContext (PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

// ✅ 3. Dependency Injection: repositories & services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();

// Stripe payment provider & payments
builder.Services.AddScoped<IPaymentProvider, StripePaymentProvider>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// ✅ 4. Password hashing
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// ✅ 5. JWT Authentication
var jwtKey = configuration["Jwt:Key"]
    ?? throw new Exception("Missing Jwt:Key configuration (check User Secrets or appsettings.json)");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidAudience = configuration["Jwt:Audience"],
        IssuerSigningKey = key
    };
});

// ✅ 6. Swagger / API setup
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ 7. Middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

