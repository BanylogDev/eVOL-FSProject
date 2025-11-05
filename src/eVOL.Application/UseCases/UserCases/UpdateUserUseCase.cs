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
    public class UpdateUserUseCase : IUpdateUserUseCase
    {
        private readonly IMySqlUnitOfWork _uow;
        private readonly IPasswordHasher _passwordHasher;

        public UpdateUserUseCase(IMySqlUnitOfWork uow, IPasswordHasher passwordHasher)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
        }

        public async Task<User?> ExecuteAsync(UpdateDTO dto)
        {

            await _uow.BeginTransactionAsync();

            try
            {
                var user = await _uow.Users.GetUserById(dto.Id);

                if (user == null)
                {
                    return null;
                }

                if (user.Password != _passwordHasher.HashPassword(dto.Password) || dto.Password != dto.ConfirmPassword)
                    return null;

                user.Name = dto.Name;
                user.Email = dto.Email;
                user.Password = _passwordHasher.HashPassword(dto.Password);

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
