using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.ChatGroupCases
{
    public class GetChatGroupByIdUseCase
    {
        private readonly IMySqlUnitOfWork _uow;

        public GetChatGroupByIdUseCase(IMySqlUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ChatGroup?> ExecuteAsync(int id)
        {
            return await _uow.ChatGroup.GetChatGroupById(id);
        }
    }
}
