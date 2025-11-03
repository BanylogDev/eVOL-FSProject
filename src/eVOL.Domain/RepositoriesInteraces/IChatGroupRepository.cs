using eVOL.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Domain.RepositoriesInteraces
{
    public interface IChatGroupRepository
    {
        Task<ChatGroup> CreateChatGroup(ChatGroup chatGroup);
        Task<ChatGroup?> DeleteChatGroup(ChatGroup chatGroup);
        Task<ChatGroup?> GetChatGroupById(int chatGroupId);
        Task<ChatGroup?> GetChatGroupByName(string chatGroupName);
        Task SaveChangesAsync();
    }
}
