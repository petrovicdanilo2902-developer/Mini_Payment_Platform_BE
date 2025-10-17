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
using PaymentPlatform.Application.Payments.PaymentPlatform.Infrastructure.Services;
using PaymentPlatform.Infrastructure.PaymentProviders;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var configuration = builder.Configuration;

// DbContext (SQLite, promeni po potrebi)
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

// DbContext (PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

// DI: repositories & services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();

builder.Services.AddScoped<IPaymentProvider, StripePaymentProvider>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Password hasher
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Authentication - JWT
var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new Exception("Jwt:Key missing"));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
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
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
