using eVOL.Application.DTOs.Requests;
using eVOL.Application.ServicesInterfaces;
using eVOL.Application.UseCases.UCInterfaces.IUserCases;
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
    public class RefreshTokenUseCase : IRefreshTokenUseCase
    {
        private readonly IJwtService _jwtService;
        private readonly IMySqlUnitOfWork _uow;
        private readonly IConfiguration _config;
        private readonly ILogger<RefreshTokenUseCase> _logger;

        public RefreshTokenUseCase(IJwtService jwtService, IMySqlUnitOfWork uow, IConfiguration config, ILogger<RefreshTokenUseCase> logger)    
        {
            _jwtService = jwtService;
            _uow = uow;
            _config = config;
            _logger = logger;
        }

        public async Task<TokenDTO?> ExecuteAsync(TokenDTO dto)
        {

            _logger.LogInformation("Starting RefreshTokenUseCase");

            await _uow.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Validating expired access token");
                var principal = _jwtService.GetPrincipalFromExpiredToken(dto.AccessToken, _config);
                if (principal == null)
                {
                    _logger.LogWarning("RefreshTokenUseCase failed: Invalid access token");
                    return null;
                }

                _logger.LogInformation("Retrieving user information from token");

                var name = principal.Identity?.Name;
                var user = await _uow.Users.GetUserByName(name);

                if (user == null || user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    _logger.LogWarning("RefreshTokenUseCase failed: Invalid refresh token or user not found");
                    return null;
                }
                 
                _logger.LogInformation("Generating new tokens for User ID: {UserId}", user.UserId);

                var newAccessToken = _jwtService.GenerateJwtToken(user, _config);
                var newRefreshToken = _jwtService.GenerateRefreshToken();

                _logger.LogInformation("Updating tokens for User ID: {UserId}", user.UserId);

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);

                _logger.LogInformation("Committing transaction for RefreshTokenUseCase");

                await _uow.CommitAsync();

                _logger.LogInformation("RefreshTokenUseCase completed successfully for User ID: {UserId}", user.UserId);

                return new TokenDTO
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                };
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "RefreshTokenUseCase failed and rolled back");
                throw;
            }
        }
    }
}
