using DnsClient.Internal;
using eVOL.Application.ServicesInterfaces;
using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using eVOL.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace eVOL.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MongoDbContext _context;
        private readonly ICacheService _cacheService;
        private readonly ILogger<MessageRepository> _logger;

        public MessageRepository(MongoDbContext context, ICacheService cacheService, ILogger<MessageRepository> logger)
        {
            _context = context;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<ChatMessage?> GetChatMessageById(int id)
        {

            var cacheKey = $"chatMessage:{id}";

            var cache = await _cacheService.GetAsync(cacheKey);

            if (cache != null)
            {
                _logger.LogInformation("Cache hit in message repository by GetChatMessageById method");
                return JsonSerializer.Deserialize<ChatMessage>(cache);
            }

            _logger.LogInformation("Cache miss in message repository by GetChatMessageById method");

            var chatMessage = await _context.ChatMessages
                .Find(x => x.MessageId == id)
                .FirstOrDefaultAsync();

            if (chatMessage != null)
            {
                await _cacheService.SetAsync(
                    cacheKey,
                    JsonSerializer.Serialize(chatMessage),
                    TimeSpan.FromMinutes(2));
            }

            return chatMessage;

        }

        public async Task<ChatMessage?> GetChatMessageBySenderId(int id)
        {
            var cacheKey = $"chatMessage:{id}";

            var cache = await _cacheService.GetAsync(cacheKey);

            if (cache != null)
            {
                _logger.LogInformation("Cache hit in message repository by GetChatMessageBySenderId method");
                return JsonSerializer.Deserialize<ChatMessage>(cache);
            }

            _logger.LogInformation("Cache miss in message repository by GetChatMessageBySenderId method");

            var chatMessage = await _context.ChatMessages
                .Find(x => x.SenderId == id)
                .FirstOrDefaultAsync();

            if (chatMessage != null)
            {
                await _cacheService.SetAsync(
                    cacheKey,
                    JsonSerializer.Serialize(chatMessage),
                    TimeSpan.FromMinutes(2));
            }

            return chatMessage;
        }

        public async Task<ChatMessage?> GetChatMessageByReceiverId(int id)
        {
            var cacheKey = $"chatMessage:{id}";

            var cache = await _cacheService.GetAsync(cacheKey);

            if (cache != null)
            {
                _logger.LogInformation("Cache hit in message repository by GetChatMessageByReceiverId method");
                return JsonSerializer.Deserialize<ChatMessage>(cache);
            }

            _logger.LogInformation("Cache miss in message repository by GetChatMessageByReceiverId method");

            var chatMessage = await _context.ChatMessages
                .Find(x => x.ReceiverId == id)
                .FirstOrDefaultAsync();

            if (chatMessage != null)
            {
                await _cacheService.SetAsync(
                    cacheKey,
                    JsonSerializer.Serialize(chatMessage),
                    TimeSpan.FromMinutes(2));
            }

            return chatMessage;
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
