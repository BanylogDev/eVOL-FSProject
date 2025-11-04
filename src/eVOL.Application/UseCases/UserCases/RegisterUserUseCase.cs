using eVOL.Application.DTOs;
using eVOL.Application.ServicesInterfaces;
using eVOL.Application.UseCases.UCInterfaces.IUserCases;
using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using eVOL.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.UserCases
{
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        private readonly IAuthRepository _authRepo;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterUserUseCase(IAuthRepository authRepo, IPasswordHasher passwordHasher)
        {
            _authRepo = authRepo;
            _passwordHasher = passwordHasher;
        }

        public async Task<User?> ExecuteAsync(RegisterDTO dto)
        {
            var existingName = await _authRepo.GetUserByName(dto.Name);
            var existingEmail = await _authRepo.GetUserByEmail(dto.Email);

            if (existingName != null || existingEmail != null)
            {
                return null;
            }

            var hashedPassword = _passwordHasher.HashPassword(dto.Password);

            var newAddress = new Address
            (
                dto.Country,
                dto.City,
                dto.AddressName,
                dto.AddressNumber
            );

            var newUser = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = hashedPassword,
                Address = newAddress,
                Role = "User",
                CreatedAt = DateTime.UtcNow,
            };

            return await _authRepo.Register(newUser);
        }

    }
}
