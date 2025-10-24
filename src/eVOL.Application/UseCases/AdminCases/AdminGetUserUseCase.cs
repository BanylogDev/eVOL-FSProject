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
    public class AdminGetUserUseCase : IAdminGetUserUseCase
    {
        private readonly IUserRepository _userRepo;

        public AdminGetUserUseCase(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<User?> ExecuteAsync(int id)
        {
            return await _userRepo.GetUserById(id);
        }
    }
}
