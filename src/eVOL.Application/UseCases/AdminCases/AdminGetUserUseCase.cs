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
        private readonly IMySqlUnitOfWork _uow;

        public AdminGetUserUseCase(IMySqlUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<User?> ExecuteAsync(int id)
        {
            return await _uow.Users.GetUserById(id);
        }
    }
}
