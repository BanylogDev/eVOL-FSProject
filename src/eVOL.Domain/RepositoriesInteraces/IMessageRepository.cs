using eVOL.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Domain.RepositoriesInteraces
{
    public interface IMessageRepository
    {
        Task<ChatMessage?> GetChatMessageById(int id);
        Task<ChatMessage?> GetChatMessageBySenderId(int id);
        Task<ChatMessage?> GetChatMessageByReceiverId(int id);
        Task<ChatMessage?> AddChatMessageToDb(ChatMessage chatMessage);
        Task<ChatMessage?> DeleteChatMessageFromDb(ChatMessage chatMessage);
    }
}
