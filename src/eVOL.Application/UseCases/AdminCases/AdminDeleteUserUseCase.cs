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
        private readonly IUserRepository _userRepo;
        private readonly IPasswordHasher _passwordHasher;

        public AdminDeleteUserUseCase(IUserRepository userRepo, IPasswordHasher passwordHasher)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
        }

        public async Task<User?> ExecuteAsync(int id)
        {
            var user = await _userRepo.GetUserById(id);

            if (user == null)
            {
                return null;
            }

            _userRepo.RemoveUser(user);
            await _userRepo.SaveChangesAsync();

            return user;
        }
    }
}
