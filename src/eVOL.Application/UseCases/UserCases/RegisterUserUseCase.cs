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
        private readonly IMySqlUnitOfWork _uow;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterUserUseCase(IMySqlUnitOfWork uow, IPasswordHasher passwordHasher)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
        }

        public async Task<User?> ExecuteAsync(RegisterDTO dto)
        {

            await _uow.BeginTransactionAsync();

            try
            {
                var existingName = await _uow.Auth.GetUserByName(dto.Name);
                var existingEmail = await _uow.Auth.GetUserByEmail(dto.Email);

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

                var newMoney = new Money(
                    dto.Balance,
                    dto.Currency);


                var newUser = new User
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Password = hashedPassword,
                    Address = newAddress,
                    Role = "User",
                    Money = newMoney,
                    CreatedAt = DateTime.UtcNow,
                };

                await _uow.Auth.Register(newUser);
                await _uow.CommitAsync();

                return newUser;
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }

        }

    }
}
