using Mapster;
using eVOL.Application.DTOs;
using eVOL.Application.DTOs.Responses;
using eVOL.Application.DTOs.Responses.User;
using eVOL.Application.ServicesInterfaces;
using eVOL.Application.UseCases.UCInterfaces.IUserCases;
using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.UserCases
{
    public class LoginUserUseCase : ILoginUserUseCase
    {
        private readonly IMySqlUnitOfWork _uow;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _config;
        private readonly ILogger<LoginUserUseCase> _logger;


        public LoginUserUseCase(IMySqlUnitOfWork uow, 
            IPasswordHasher passwordHasher, 
            IJwtService jwtService, 
            IConfiguration config, 
            ILogger<LoginUserUseCase> logger)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _config = config;
            _logger = logger;
        }

        public async Task<LoginUserResponse?> ExecuteAsync(LoginDTO dto)
        {

            _logger.LogInformation("Starting LoginUserUseCase for Email: {Email}", dto.Email);

            await _uow.BeginTransactionAsync();

            try
            {
                var user = await _uow.Auth.GetUserByEmail(dto.Email);

                if (user == null || !_passwordHasher.VerifyPassword(dto.Password, user.Password))
                {
                    _logger.LogWarning("LoginUserUseCase failed: User not found or invalid password for Email: {Email}", dto.Email);
                    return null;
                }

                _logger.LogInformation("Generating tokens for User ID: {UserId}", user.UserId);

                var token = _jwtService.GenerateJwtToken(user, _config);
                var refreshToken = _jwtService.GenerateRefreshToken();

                _logger.LogInformation("Updating tokens for User ID: {UserId}", user.UserId);

                user.RefreshToken = refreshToken;
                user.AccessToken = token;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);

                await _uow.CommitAsync();

                _logger.LogInformation("LoginUserUseCase completed successfully for User ID: {UserId}", user.UserId);

                _logger.LogInformation($"handled by {Environment.MachineName}");

                return user.Adapt<LoginUserResponse>();
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "LoginUserUseCase failed and rolled back for Email: {Email}", dto.Email);
                throw;
            }

        } 
    }
}
