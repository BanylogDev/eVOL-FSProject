using eVOL.Application.Mappings;
using eVOL.Application.Messaging.Interfaces;
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
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Text;
using System.Threading.RateLimiting;

namespace eVOL.API.Configuration
{
    public static class DependecyInjection
    {

        public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
        {

            var redisConnectionString = configuration["CacheSettings:RedisConnection"];
            var options = ConfigurationOptions.Parse(redisConnectionString);
            options.AbortOnConnectFail = false;

            services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(options)
            );

            services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(options)
            );

            var redisConnection = configuration["CacheSettings:RedisConnection"];
            services.AddSignalR().AddStackExchangeRedis(redisConnection, opts =>
            {
                opts.Configuration.ChannelPrefix = "evol.signalr";
            });

            return services;
        }

        public static IServiceCollection AddDatabases(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    configuration.GetConnectionString("DefaultConnection"),
                    Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect(
                        configuration.GetConnectionString("DefaultConnection")
                    )
                )
            );

            services.AddSingleton<MongoDbContext>(sp =>
            {
                var config = configuration;
                var connectionString = config.GetConnectionString("MongoDB");
                return new MongoDbContext(connectionString, "eVOLChat");
            });

            services.AddScoped<IClientSessionHandle>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                return client.StartSession();
            });

            return services;
        }

        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }

        public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                    };
            });


            return services;
        }

        public static IServiceCollection AddCorsService(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyMethod()
                          .AllowAnyHeader()
                          .WithOrigins("http://localhost:5001")
                          .AllowCredentials();
                });
            });

            return services;
        }

        public static IServiceCollection AddMapper(this IServiceCollection services)
        {
            var config = new TypeAdapterConfig();
            config.Scan(typeof(UserMappings).Assembly);

            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();

            return services;
        }

        public static IServiceCollection AddScopedUseCases(this IServiceCollection services)
        {
            // Services

            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IAesService, AesService>();

            // Messaging

            services.AddScoped<IRabbitMqPublisher, RabbitMqPublisher>();

            // Admin Use Cases

            services.AddScoped<IAdminDeleteUserUseCase, AdminDeleteUserUseCase>();
            services.AddScoped<IAdminGetUserUseCase, AdminGetUserUseCase>();

            // User Use Cases

            services.AddScoped<IDeleteUserUseCase, DeleteUserUseCase>();
            services.AddScoped<IGetUserUseCase, GetUserUseCase>();
            services.AddScoped<ILoginUserUseCase, LoginUserUseCase>();
            services.AddScoped<IRefreshTokenUseCase, RefreshTokenUseCase>();
            services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
            services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();

            // Chat Group Use Cases

            services.AddScoped<IAddUserToChatGroupUseCase, AddUserToChatGroupUseCase>();
            services.AddScoped<ICreateChatGroupUseCase, CreateChatGroupUseCase>();
            services.AddScoped<IDeleteChatGroupUseCase, DeleteChatGroupUseCase>();
            services.AddScoped<IGetChatGroupByIdUseCase, GetChatGroupByIdUseCase>();
            services.AddScoped<ILeaveChatGroupUseCase, LeaveChatGroupUseCase>();
            services.AddScoped<IRemoveUserFromChatGroupUseCase, RemoveUserFromChatGroupUseCase>();
            services.AddScoped<ISendChatGroupMessageUseCase, SendChatGroupMessageUseCase>();
            services.AddScoped<ITransferOwnershipOfChatGroupUseCase, TransferOwnershipOfChatGroupUseCase>();

            // Support Ticket Use Cases

            services.AddScoped<IClaimSupportTicketUseCase, ClaimSupportTicketUseCase>();
            services.AddScoped<ICreateSupportTicketUseCase, CreateSupportTicketUseCase>();
            services.AddScoped<IDeleteSupportTicketUseCase, DeleteSupportTicketUseCase>();
            services.AddScoped<IGetSupportTicketByIdUseCase, GetSupportTicketByIdUseCase>();
            services.AddScoped<ISendSupportTicketMessageUseCase, SendSupportTicketMessageUseCase>();
            services.AddScoped<IUnClaimSupportTicketUseCase, UnClaimSupportTicketUseCase>();


            // Repositories

            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IChatGroupRepository, ChatGroupRepository>();
            services.AddScoped<ISupportTicketRepository, SupportTicketRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();

            // Unit's Of Work

            services.AddScoped<IMySqlUnitOfWork, MySqlUnitOfWork>();
            services.AddScoped<IMongoUnitOfWork, MongoUnitOfWork>();



            return services;
        }

        public static IServiceCollection AddRateLimiterService(this IServiceCollection services)
        {
            services.AddRateLimiter(o =>
            {

                o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                o.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                             ?? context.Connection.RemoteIpAddress?.ToString()
                             ?? "unknown";

                    if (!string.IsNullOrEmpty(ip) && ip.Contains(','))
                    {
                        ip = ip.Split(',')[0].Trim();
                    }


                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ip,
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 60,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        });
                });
            });

            return services;
        }
    }

    
}
