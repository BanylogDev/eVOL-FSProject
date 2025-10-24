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
        private readonly IAuthRepository _authRepo;
        private readonly IConfiguration _config;

        public RefreshTokenUseCase(IJwtService jwtService, IAuthRepository authRepo, IConfiguration config)
        {
            _jwtService = jwtService;
            _authRepo = authRepo;
            _config = config;
        }

        public async Task<TokenDTO?> ExecuteAsync(TokenDTO dto)
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(dto.AccessToken, _config);
            if (principal == null)
                return null;

            var name = principal.Identity?.Name;
            var user = await _authRepo.GetUserByName(name);

            if (user == null || user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return null;

            var newAccessToken = _jwtService.GenerateJwtToken(user, _config);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);

            await _authRepo.SaveChangesAsync();

            return new TokenDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            };
        }
    }
}
