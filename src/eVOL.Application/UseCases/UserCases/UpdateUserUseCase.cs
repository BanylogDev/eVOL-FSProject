using eVOL.Application.DTOs.Requests;
using eVOL.Application.ServicesInterfaces;
using eVOL.Application.UseCases.UCInterfaces.IUserCases;
using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.UserCases
{
    public class UpdateUserUseCase : IUpdateUserUseCase
    {
        private readonly IMySqlUnitOfWork _uow;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<UpdateUserUseCase> _logger;

        public UpdateUserUseCase(IMySqlUnitOfWork uow, IPasswordHasher passwordHasher, ILogger<UpdateUserUseCase> logger)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<User?> ExecuteAsync(UpdateDTO dto)
        {

            _logger.LogInformation("Starting UpdateUserUseCase for User ID: {UserId}", dto.Id);

            await _uow.BeginTransactionAsync();

            try
            {
                var user = await _uow.Users.GetUserById(dto.Id);

                if (user == null)
                {
                    _logger.LogWarning("UpdateUserUseCase failed: User not found.");
                    return null;
                }

                if (user.Password != _passwordHasher.HashPassword(dto.Password) || dto.Password != dto.ConfirmPassword)
                {
                    _logger.LogWarning("UpdateUserUseCase failed: Password mismatch.");
                    return null;
                }

                _logger.LogInformation("Updating User ID: {UserId}", dto.Id);

                user.Name = dto.Name;
                user.Email = dto.Email;
                user.Password = _passwordHasher.HashPassword(dto.Password);

                await _uow.CommitAsync();

                _logger.LogInformation("UpdateUserUseCase completed successfully for User ID: {UserId}", dto.Id);

                return user;
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "UpdateUserUseCase failed and rolled back for User ID: {UserId}", dto.Id);
                throw;
            }

        }
    }
}
