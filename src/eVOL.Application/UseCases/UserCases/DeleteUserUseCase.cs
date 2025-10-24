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
        private readonly IUserRepository _userRepo;
        private readonly IPasswordHasher _passwordHasher;

        public DeleteUserUseCase(IUserRepository userRepo, IPasswordHasher passwordHasher)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
        }

        public async Task<User?> ExecuteAsync(DeleteAccountDTO dto)
        {
            var user = await _userRepo.GetUserById(dto.Id);

            if (user == null || 
                dto.Password != dto.ConfirmPassword ||
               _passwordHasher.VerifyPassword(dto.Password, _passwordHasher.HashPassword(dto.Password)))
            {
                return null;
            }

            _userRepo.RemoveUser(user);
            await _userRepo.SaveChangesAsync();

            return user;
        }


    }
}
