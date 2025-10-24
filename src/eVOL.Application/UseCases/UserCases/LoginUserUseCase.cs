using eVOL.Application.DTOs;
using eVOL.Application.ServicesInterfaces;
using eVOL.Application.UseCases.UCInterfaces.IUserCases;
using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.UserCases
{
    public class LoginUserUseCase : ILoginUserUseCase
    {
        private readonly IAuthRepository _authRepo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _config;

        
        public LoginUserUseCase(IAuthRepository authRepo, IPasswordHasher passwordHasher, IJwtService jwtService, IConfiguration config)
        {
            _authRepo = authRepo;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _config = config;
        }

        public async Task<User?> ExecuteAsync(LoginDTO dto)
        {
            var user = await _authRepo.GetUserByEmail(dto.Email);

            if (user == null || !_passwordHasher.VerifyPassword(dto.Password, user.Password))
            {
                return null;
            }

            var token = _jwtService.GenerateJwtToken(user, _config);
            var refreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.AccessToken = token;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);

            await _authRepo.SaveChangesAsync();

            return user;
        } 
    }
}
