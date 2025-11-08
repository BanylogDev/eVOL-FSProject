using eVOL.Application.UseCases.UCInterfaces.IChatGroupCases;
using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<SendChatGroupMessageUseCase> _logger;

        public SendChatGroupMessageUseCase(IMongoUnitOfWork mongouow, IMySqlUnitOfWork mysqluow, ILogger<SendChatGroupMessageUseCase> logger)
        {
            _mongouow = mongouow;
            _mysqluow = mysqluow;
            _logger = logger;
        }

        public async Task<(ChatMessage?, User?)> ExecuteAsync(string message, string chatGroupName, int userId)
        {

            _logger.LogInformation("Started sending message from user with id: {UserId} to chat group with name: {ChatGroupName}, Text: {Text}", userId, chatGroupName, message);

            _mongouow.BeginTransactionAsync();
            await _mysqluow.BeginTransactionAsync();

            try
            {

                var chatGroup = await _mysqluow.ChatGroup.GetChatGroupByName(chatGroupName);

                var user = await _mysqluow.Users.GetUserById(userId);

                if (chatGroup == null || user == null)
                {
                    _logger.LogWarning("Chat group with name: {ChatGroupName} or user with id: {UserId} weren't found!", chatGroupName, userId);
                    return (null,null);
                }

                var newMessage = new ChatMessage
                {
                    Text = message,
                    SenderId = user.UserId,
                    ReceiverId = chatGroup.Id,
                    CreatedAt = DateTime.UtcNow,
                };

                _logger.LogInformation("Adding custom message to mongo database!");

                await _mongouow.Message.AddChatMessageToDb(newMessage);

                _logger.LogInformation("Finished adding custom message to mongo database!");

                await _mongouow.CommitAsync();
                await _mysqluow.CommitAsync();

                _logger.LogInformation("Ended sending message from user with id: {UserId} to chat group with name: {ChatGroupName}, Text: {Text}, Success!", userId, chatGroupName, message);

                return (newMessage, user);
            }
            catch (Exception ex) 
            {
                await _mongouow.RollbackAsync();
                await _mysqluow.RollbackAsync();
                _logger.LogError(ex, "Error, Something went wrong while sending message to chat group with name: {ChatGroupName}", chatGroupName);
                throw;
            }
        }
    }
}
