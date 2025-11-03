using eVOL.Application.DTOs;
using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.ChatGroupCases
{
    public class DeleteChatGroupUseCase
    {
        private readonly IChatGroupRepository _chatGroupRepo;

        public DeleteChatGroupUseCase(IChatGroupRepository chatGroupRepo)
        {
            _chatGroupRepo = chatGroupRepo;
        }

        public async Task<ChatGroup?> ExecuteAsync(int id)
        {
            var chatGroup = await _chatGroupRepo.GetChatGroupById(id);

            if (chatGroup == null)
            {
                return null;
            }

            return await _chatGroupRepo.CreateChatGroup(chatGroup);
        }
    }
}
