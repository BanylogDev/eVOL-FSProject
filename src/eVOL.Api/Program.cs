using eVOL.API.Hubs;
using eVOL.Application.ServicesInterfaces;
using eVOL.Application.UseCases.AdminCases;
using eVOL.Application.UseCases.ChatGroupCases;
using eVOL.Application.UseCases.SupportTicketCases;
using eVOL.Application.UseCases.UCInterfaces.IAdminCases;
using eVOL.Application.UseCases.UCInterfaces.IChatGroupCases;
using eVOL.Application.UseCases.UCInterfaces.ISupportTicketCases;
using eVOL.Application.UseCases.UCInterfaces.IUserCases;
using eVOL.Application.UseCases.UserCases;
using eVOL.Domain.RepositoriesInteraces;
using eVOL.Infrastructure.Data;
using eVOL.Infrastructure.Persistence;
using eVOL.Infrastructure.Repositories;
using eVOL.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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
        policy.AllowAnyMethod()
              .AllowAnyHeader()  
              .WithOrigins("http://localhost:5001")
              .AllowCredentials();
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

// Chat Group Use Cases

builder.Services.AddScoped<IAddUserToChatGroupUseCase, AddUserToChatGroupUseCase>();
builder.Services.AddScoped<ICreateChatGroupUseCase, CreateChatGroupUseCase>();
builder.Services.AddScoped<IDeleteChatGroupUseCase, DeleteChatGroupUseCase>();
builder.Services.AddScoped<IGetChatGroupByIdUseCase, GetChatGroupByIdUseCase>();
builder.Services.AddScoped<ILeaveChatGroupUseCase, LeaveChatGroupUseCase>();
builder.Services.AddScoped<IRemoveUserFromChatGroupUseCase, RemoveUserFromChatGroupUseCase>();
builder.Services.AddScoped<ISendChatGroupMessageUseCase, SendChatGroupMessageUseCase>();
builder.Services.AddScoped<ITransferOwnershipOfChatGroupUseCase, TransferOwnershipOfChatGroupUseCase>();

// Support Ticket Use Cases

builder.Services.AddScoped<IClaimSupportTicketUseCase, ClaimSupportTicketUseCase>();
builder.Services.AddScoped<ICreateSupportTicketUseCase, CreateSupportTicketUseCase>();
builder.Services.AddScoped<IDeleteSupportTicketUseCase, DeleteSupportTicketUseCase>();
builder.Services.AddScoped<IGetSupportTicketByIdUseCase, GetSupportTicketByIdUseCase>();
builder.Services.AddScoped<IUnClaimSupportTicketUseCase, UnClaimSupportTicketUseCase>();


// Repositories

builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IChatGroupRepository, ChatGroupRepository>();
builder.Services.AddScoped<ISupportTicketRepository, SupportTicketRepository>();

// Unit's Of Work

builder.Services.AddScoped<IMySqlUnitOfWork, MySqlUnitOfWork>();
builder.Services.AddScoped<IMongoUnitOfWork, MongoUnitOfWork>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chat-hub");

app.Run();
