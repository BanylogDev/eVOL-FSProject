using eVOL.Application.DTOs;
using eVOL.Application.ServicesInterfaces;
using eVOL.Application.UseCases.UCInterfaces.IUserCases;
using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
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

        public DeleteUserUseCase(IMySqlUnitOfWork uow, IPasswordHasher passwordHasher)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
        }

        public async Task<User?> ExecuteAsync(DeleteAccountDTO dto)
        {

            await _uow.BeginTransactionAsync();

            try
            {
                var user = await _uow.Users.GetUserById(dto.Id);

                if (user == null ||
                    dto.Password != dto.ConfirmPassword ||
                   _passwordHasher.VerifyPassword(dto.Password, _passwordHasher.HashPassword(dto.Password)))
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
