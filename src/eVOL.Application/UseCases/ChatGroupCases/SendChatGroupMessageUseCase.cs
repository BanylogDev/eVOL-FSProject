using eVOL.Application.UseCases.UCInterfaces.IChatGroupCases;
using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.ChatGroupCases
{
    public class SendChatGroupMessageUseCase : ISendChatGroupMessageUseCase
    {
        private readonly IMongoUnitOfWork _mongouow;
        private readonly IMySqlUnitOfWork _mysqluow;

        public SendChatGroupMessageUseCase(IMongoUnitOfWork mongouow, IMySqlUnitOfWork mysqluow)
        {
            _mongouow = mongouow;
            _mysqluow = mysqluow;
        }

        public async Task<(ChatMessage?, User?)> ExecuteAsync(string message, string chatGroupName, int userId)
        {
            _mongouow.BeginTransactionAsync();
            await _mysqluow.BeginTransactionAsync();

            try
            {

                var chatGroup = await _mysqluow.ChatGroup.GetChatGroupByName(chatGroupName);

                var user = await _mysqluow.Users.GetUserById(userId);

                if (chatGroup == null || user == null)
                {
                    return (null,null);
                }

                var newMessage = new ChatMessage
                {
                    Text = message,
                    SenderId = user.UserId,
                    ReceiverId = chatGroup.Id,
                    CreatedAt = DateTime.UtcNow,
                };

                await _mongouow.Message.AddChatMessageToDb(newMessage);

                await _mongouow.CommitAsync();
                await _mysqluow.CommitAsync();

                return (newMessage, user);
            }
            catch
            {
                await _mongouow.RollbackAsync();
                await _mysqluow.RollbackAsync();
                throw;
            }
        }
    }
}
