using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using eVOL.Infrastructure.Data;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MongoDbContext _context;

        public MessageRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<ChatMessage?> GetChatMessageById(int id)
        {
            return await _context.ChatMessages.Find(x => x.MessageId == id).FirstOrDefaultAsync();
        }

        public async Task<ChatMessage?> GetChatMessageBySenderId(int id)
        {
            return await _context.ChatMessages.Find(x => x.SenderId == id).FirstOrDefaultAsync();
        }

        public async Task<ChatMessage?> GetChatMessageByReceiverId(int id)
        {
            return await _context.ChatMessages.Find(x => x.ReceiverId == id).FirstOrDefaultAsync();
        }

        public async Task<ChatMessage?> AddChatMessageToDb(ChatMessage chatMessage)
        {
            await _context.ChatMessages.InsertOneAsync(chatMessage);

            return chatMessage;
        }

        public async Task<ChatMessage?> DeleteChatMessageFromDb(ChatMessage chatMessage)
        {
            await _context.ChatMessages.DeleteOneAsync(x => x.MessageId == chatMessage.MessageId);

            return chatMessage;
        }
    }
}
