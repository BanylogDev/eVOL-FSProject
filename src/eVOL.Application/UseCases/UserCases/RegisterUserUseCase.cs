using AutoMapper;
using eVOL.Application.DTOs.Requests;
using eVOL.Application.DTOs.Responses.User;
using eVOL.Application.ServicesInterfaces;
using eVOL.Application.UseCases.UCInterfaces.IUserCases;
using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using eVOL.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<RegisterUserUseCase> _logger;
        private readonly IMapper _mapper;

        public RegisterUserUseCase(IMySqlUnitOfWork uow, IPasswordHasher passwordHasher, ILogger<RegisterUserUseCase> logger, IMapper mapper)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<RegisterUserResponse?> ExecuteAsync(RegisterDTO dto)
        {

            _logger.LogInformation("Starting RegisterUserUseCase for Name: {Name}, Email: {Email}", dto.Name, dto.Email);

            await _uow.BeginTransactionAsync();

            try
            {
                var existingName = await _uow.Auth.GetUserByName(dto.Name);
                var existingEmail = await _uow.Auth.GetUserByEmail(dto.Email);

                if (existingName != null || existingEmail != null)
                {
                    _logger.LogWarning("RegisterUserUseCase failed: Name or Email already exists. Name: {Name}, Email: {Email}", dto.Name, dto.Email);
                    return null;
                }


                _logger.LogInformation("Hashing password for Name: {Name}", dto.Name);

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

                _logger.LogInformation("Registering new user: Name: {Name}, Email: {Email}", dto.Name, dto.Email);

                await _uow.Auth.Register(newUser);
                await _uow.CommitAsync();

                _logger.LogInformation("RegisterUserUseCase completed successfully for Name: {Name}, Email: {Email}", dto.Name, dto.Email);

                return _mapper.Map<RegisterUserResponse>(newUser);
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "RegisterUserUseCase failed and rolled back for Name: {Name}, Email: {Email}", dto.Name, dto.Email);
                throw;
            }

        }

    }
}
