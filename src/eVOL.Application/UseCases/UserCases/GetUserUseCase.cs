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
    public class GetUserUseCase : IGetUserUseCase
    {
        private readonly IMySqlUnitOfWork _uow;

        public GetUserUseCase(IMySqlUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<User?> ExecuteAsync(int id)
        {
            return await _uow.Users.GetUserById(id);
        }
    }
}
