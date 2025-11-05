using eVOL.Application.DTOs;
using eVOL.Application.ServicesInterfaces;
using eVOL.Application.UseCases.UCInterfaces.IAdminCases;
using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
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

        public AdminDeleteUserUseCase(IMySqlUnitOfWork uow, IPasswordHasher passwordHasher)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
        }

        public async Task<User?> ExecuteAsync(int id)
        {

            await _uow.BeginTransactionAsync();

            try
            {
                var user = await _uow.Users.GetUserById(id);

                if (user == null)
                {
                    return null;
                }

                _uow.Users.RemoveUser(user);
                await _uow.CommitAsync();

                return user;
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }
    }
}
