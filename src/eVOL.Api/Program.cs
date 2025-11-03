using eVOL.API.Hubs;
using eVOL.Application.ServicesInterfaces;
using eVOL.Application.UseCases.AdminCases;
using eVOL.Application.UseCases.UCInterfaces.IAdminCases;
using eVOL.Application.UseCases.UCInterfaces.IUserCases;
using eVOL.Application.UseCases.UserCases;
using eVOL.Domain.RepositoriesInteraces;
using eVOL.Infrastructure.Data;
using eVOL.Infrastructure.Repositories;
using eVOL.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);
builder.Services.AddSingleton<MongoDbContext>(sp =>
{
    var config = builder.Configuration;
    var connectionString = config.GetConnectionString("MongoDB");
    return new MongoDbContext(connectionString, "eVOLChat");
});


var redisConnectionString = builder.Configuration["CacheSettings:RedisConnection"];
var options = ConfigurationOptions.Parse(redisConnectionString);
options.AbortOnConnectFail = false;

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(options)
);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
{
options.TokenValidationParameters = new TokenValidationParameters
{
ValidateIssuer = true,
ValidateAudience = true,
ValidateLifetime = true,
ValidateIssuerSigningKey = true,
ValidIssuer = builder.Configuration["Jwt:Issuer"],
ValidAudience = builder.Configuration["Jwt:Audience"],
IssuerSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
};
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
{
    Title = "Zelta",
    Version = "v1"
});

options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
{
    Name = "Authorization",
    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
    Description = "Enter 'Bearer' [space] and then your valid token.\n\nExample: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6...'"
});

options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var redisConnection = builder.Configuration["CacheSettings:RedisConnection"];
builder.Services.AddSignalR().AddStackExchangeRedis(redisConnection, opts =>
{
    opts.Configuration.ChannelPrefix = "evol.signalr";
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyMethod() // Allow POST, GET, etc.
              .AllowAnyHeader()  // Allow necessary headers
              .WithOrigins("http://localhost:5001") // <-- IMPORTANT: Replace with your Blazor app's URL
              .AllowCredentials(); // Crucial for SignalR authentication
    });
});

// Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IAesService, AesService>();

// Admin Use Cases

builder.Services.AddScoped<IAdminDeleteUserUseCase, AdminDeleteUserUseCase>();
builder.Services.AddScoped<IAdminGetUserUseCase, AdminGetUserUseCase>();

// User Use Cases

builder.Services.AddScoped<IDeleteUserUseCase, DeleteUserUseCase>();
builder.Services.AddScoped<IGetUserUseCase, GetUserUseCase>();
builder.Services.AddScoped<ILoginUserUseCase, LoginUserUseCase>();
builder.Services.AddScoped<IRefreshTokenUseCase, RefreshTokenUseCase>();
builder.Services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
builder.Services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();

// Repositories

builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// --- 3. Use CORS and SignalR Middleware ---
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapControllers();
app.MapHub<ChatHub>("/chat-hub");

app.Run();
