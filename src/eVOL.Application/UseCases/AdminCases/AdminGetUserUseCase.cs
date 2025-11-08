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
    public class AdminGetUserUseCase : IAdminGetUserUseCase
    {
        private readonly IMySqlUnitOfWork _uow;
        private readonly ILogger<AdminGetUserUseCase> _logger;

        public AdminGetUserUseCase(IMySqlUnitOfWork uow, ILogger<AdminGetUserUseCase> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<User?> ExecuteAsync(int id)
        {

            _logger.LogInformation("Admin -> Started geting user with id: {UserId}", id);

            var user = await _uow.Users.GetUserById(id);

            if (user == null)
            {
                _logger.LogWarning("Admin -> User with id: {UserId} not found! ", id);
                return null;
            }

            _logger.LogInformation("Admin -> Ended getting user with id: {UserId}, Success", id);
            return user;
        }
    }
}
