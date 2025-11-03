using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.ChatGroupCases
{
    public class AddUserToChatGroupUseCase
    {
        private readonly IChatGroupRepository _chatGroupRepo;
        private readonly IUserRepository _userRepo;

        public AddUserToChatGroupUseCase(IChatGroupRepository chatGroupRepo, IUserRepository userRepo)
        {
            _chatGroupRepo = chatGroupRepo;
            _userRepo = userRepo;
        }

        public async Task<User?> ExecuteAsync(int userId, string chatGroupName)
        {
            var user = await _userRepo.GetUserById(userId);

            var chatGroup = await _chatGroupRepo.GetChatGroupByName(chatGroupName);

            if (user == null || chatGroup == null || chatGroup.GroupUsers.Contains(user))
            {
                return null;
            }

            chatGroup.GroupUsers.Add(user);
            chatGroup.TotalUsers += 1;

            await _chatGroupRepo.SaveChangesAsync();

            return user;
        }
    }
}
