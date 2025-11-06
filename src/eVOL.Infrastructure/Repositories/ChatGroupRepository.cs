using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using eVOL.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Infrastructure.Repositories
{
    public class ChatGroupRepository : IChatGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatGroupRepository (ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ChatGroup> CreateChatGroup(ChatGroup chatGroup)
        {
            await _context.ChatGroups.AddAsync(chatGroup);

            return chatGroup;
        }

        public ChatGroup? DeleteChatGroup(ChatGroup chatGroup)
        {

            _context.ChatGroups.Remove(chatGroup);

            return chatGroup;
        }

        public async Task<ChatGroup?> GetChatGroupById(int chatGroupId)
        {
            return await _context.ChatGroups.FindAsync(chatGroupId);
        }

        public async Task<ChatGroup?> GetChatGroupByName(string chatGroupName)
        {
            return await _context.ChatGroups.FirstOrDefaultAsync(c => c.Name == chatGroupName);
        }

    }
}
