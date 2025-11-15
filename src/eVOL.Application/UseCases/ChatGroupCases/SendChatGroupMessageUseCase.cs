using eVOL.Application.Messaging.Interfaces;
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
        private readonly IRabbitMqPublisher _publisher;
        private readonly IMySqlUnitOfWork _mysqluow;
        private readonly ILogger<SendChatGroupMessageUseCase> _logger;

        public SendChatGroupMessageUseCase(IRabbitMqPublisher publisher, IMySqlUnitOfWork mysqluow, ILogger<SendChatGroupMessageUseCase> logger)
        {
            _publisher = publisher;
            _mysqluow = mysqluow;
            _logger = logger;
        }

        public async Task<(ChatMessage?, User?)> ExecuteAsync(string message, string chatGroupName, int userId)
        {

            _logger.LogInformation("Started sending message from user with id: {UserId} to chat group with name: {ChatGroupName}, Text: {Text}", userId, chatGroupName, message);

            await _mysqluow.BeginTransactionAsync();

            try
            {
                var chatGroup = await _mysqluow.ChatGroup.GetChatGroupByName(chatGroupName);
                var user = await _mysqluow.Users.GetUserById(userId);

                if (chatGroup == null || user == null) return (null, null);

                var evt = new ChatMessage
                {
                    Text = message,
                    SenderId = user.UserId,
                    ReceiverId = chatGroup.Id,
                    CreatedAt = DateTime.UtcNow
                };

                await _publisher.PublishAsync(evt);

                await _mysqluow.CommitAsync();

                return (new ChatMessage { Text = evt.Text, SenderId = evt.SenderId, ReceiverId = evt.ReceiverId, CreatedAt = evt.CreatedAt }, user);
            }
            catch (Exception ex) 
            {
                await _mysqluow.RollbackAsync();
                _logger.LogError(ex, "Error, Something went wrong while sending message to chat group with name: {ChatGroupName}", chatGroupName);
                throw;
            }
        }
    }
}
