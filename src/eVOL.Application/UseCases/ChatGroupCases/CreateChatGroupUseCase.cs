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
    public class CreateChatGroupUseCase
    {
        private readonly IChatGroupRepository _chatGroupRepo;

        public CreateChatGroupUseCase(IChatGroupRepository chatGroupRepo)
        {
            _chatGroupRepo = chatGroupRepo;
        }

        public async Task<ChatGroup> ExecuteAsync(ChatGroupDTO dto)
        {
            var chatGroup = new ChatGroup
            {
                Name = dto.Name,
                TotalUsers = dto.TotalUsers,
                GroupUsers = dto.GroupUsers,
                OwnerId = dto.OwnerId,
                CreatedAt = DateTime.UtcNow,
            };

            return await _chatGroupRepo.CreateChatGroup(chatGroup);
        }
    }
}
