using eVOL.Application.DTOs;
using eVOL.Application.ServicesInterfaces;
using eVOL.Application.UseCases.UCInterfaces.IAdminCases;
using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.AdminCases
{
    public class AdminDeleteUserUseCase : IAdminDeleteUserUseCase
    {
        private readonly IMySqlUnitOfWork _uow;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<AdminDeleteUserUseCase> _logger;

        public AdminDeleteUserUseCase(IMySqlUnitOfWork uow, IPasswordHasher passwordHasher, ILogger<AdminDeleteUserUseCase> logger)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<User?> ExecuteAsync(int id)
        {

            _logger.LogInformation("Admin -> Started Deletion of user {UserId}", id);

            await _uow.BeginTransactionAsync();

            try
            {
                var user = await _uow.Users.GetUserById(id);

                if (user == null)
                {

                    _logger.LogWarning("Admin -> Error, user doesn't exist with id: {UserId}", id);

                    return null;
                }

                _logger.LogInformation("Admin -> Deleting user with id: {UserId}", id);

                _uow.Users.RemoveUser(user);
                await _uow.CommitAsync();

                _logger.LogInformation("Admin -> Success, Ended Deletion of user {UserId}", id);

                return user;
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "Admin -> Error, Something went wrong during deletion of user with id: {UserId}", id);
                throw;
            }
        }
    }
}
