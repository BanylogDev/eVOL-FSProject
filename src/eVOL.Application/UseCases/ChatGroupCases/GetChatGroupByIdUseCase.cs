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
        private readonly IChatGroupRepository _chatGroupRepo;

        public GetChatGroupByIdUseCase(IChatGroupRepository chatGroupRepo)
        {
            _chatGroupRepo = chatGroupRepo;
        }

        public async Task<ChatGroup?> ExecuteAsync(int id)
        {
            return await _chatGroupRepo.GetChatGroupById(id);
        }
    }
}
