using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.ChatGroupCases
{
    public class RemoveUserFromChatGroupUseCase
    {
        private readonly IChatGroupRepository _chatGroupRepo;
        private readonly IUserRepository _userRepo;

        public RemoveUserFromChatGroupUseCase(IChatGroupRepository chatGroupRepo, IUserRepository userRepo)
        {
            _chatGroupRepo = chatGroupRepo;
            _userRepo = userRepo;
        }

        public async Task<User?> ExecuteAsync(int userId, int chatGroupId)
        {
            var user = await _userRepo.GetUserById(userId);

            var chatGroup = await _chatGroupRepo.GetChatGroupById(chatGroupId);

            if (user == null || chatGroup == null || chatGroup.GroupUsers.Contains(user))
            {
                return null;
            }

            chatGroup.GroupUsers.Remove(user);
            chatGroup.TotalUsers -= 1;

            await _chatGroupRepo.SaveChangesAsync();

            return user;
        }
    }
}
