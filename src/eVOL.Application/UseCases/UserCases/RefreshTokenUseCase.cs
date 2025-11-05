using eVOL.Application.DTOs;
using eVOL.Application.ServicesInterfaces;
using eVOL.Application.UseCases.UCInterfaces.IUserCases;
using eVOL.Domain.RepositoriesInteraces;
using Microsoft.Extensions.Configuration;
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

        public RefreshTokenUseCase(IJwtService jwtService, IMySqlUnitOfWork uow, IConfiguration config)
        {
            _jwtService = jwtService;
            _uow = uow;
            _config = config;
        }

        public async Task<TokenDTO?> ExecuteAsync(TokenDTO dto)
        {

            await _uow.BeginTransactionAsync();

            try
            {
                var principal = _jwtService.GetPrincipalFromExpiredToken(dto.AccessToken, _config);
                if (principal == null)
                    return null;

                var name = principal.Identity?.Name;
                var user = await _uow.Auth.GetUserByName(name);

                if (user == null || user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                    return null;

                var newAccessToken = _jwtService.GenerateJwtToken(user, _config);
                var newRefreshToken = _jwtService.GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);

                await _uow.CommitAsync();

                return new TokenDTO
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                };
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }
    }
}
