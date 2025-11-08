using eVOL.Application.DTOs;
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
    public class DeleteUserUseCase : IDeleteUserUseCase
    {
        private readonly IMySqlUnitOfWork _uow;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<DeleteUserUseCase> _logger;

        public DeleteUserUseCase(IMySqlUnitOfWork uow, IPasswordHasher passwordHasher, ILogger<DeleteUserUseCase> logger)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<User?> ExecuteAsync(DeleteAccountDTO dto)
        {

            _logger.LogInformation("Starting DeleteUserUseCase for User ID: {UserId}", dto.Id);

            await _uow.BeginTransactionAsync();

            try
            {
                var user = await _uow.Users.GetUserById(dto.Id);

                if (user == null ||
                    dto.Password != dto.ConfirmPassword ||
                   _passwordHasher.VerifyPassword(dto.Password, _passwordHasher.HashPassword(dto.Password)))
                {
                    _logger.LogWarning("DeleteUserUseCase failed: User not found or password mismatch.");
                    return null;
                }

                _logger.LogInformation("Deleting User ID: {UserId}", dto.Id);

                _uow.Users.RemoveUser(user);
                await _uow.CommitAsync();

                _logger.LogInformation("DeleteUserUseCase completed successfully for User ID: {UserId}", dto.Id);

                return user;
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "DeleteUserUseCase failed and rolled back for User ID: {UserId}", dto.Id); 
                throw;
            }

        }


    }
}
