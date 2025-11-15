using eVOL.Application.Messaging.Interfaces;
using eVOL.Application.UseCases.UCInterfaces.IChatGroupCases;
using eVOL.Application.UseCases.UCInterfaces.ISupportTicketCases;
using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.SupportTicketCases
{
    public class SendSupportTicketMessageUseCase : ISendSupportTicketMessageUseCase
    {
        private readonly IRabbitMqPublisher _publisher;
        private readonly IMySqlUnitOfWork _mysqluow;
        private readonly ILogger<SendSupportTicketMessageUseCase> _logger;

        public SendSupportTicketMessageUseCase(IRabbitMqPublisher publisher, IMySqlUnitOfWork mysqluow, ILogger<SendSupportTicketMessageUseCase> logger)
        {
            _publisher = publisher;
            _mysqluow = mysqluow;
            _logger = logger;
        }

        public async Task<(ChatMessage?, User?)> ExecuteAsync(string message, string supportTicketName, int userId)
        {

            _logger.LogInformation("Started sending message from user with id: {UserId} to support ticket with name: {SupportTicketName}, Text: {Text}", userId, supportTicketName, message);

            await _mysqluow.BeginTransactionAsync();

            try
            {

                var supportTicket = await _mysqluow.SupportTicket.GetSupportTicketByName(supportTicketName);

                var user = await _mysqluow.Users.GetUserById(userId);

                if (supportTicket == null || user == null)
                {
                    _logger.LogWarning("Support ticket with name: {SupportTicketName} or user with id: {UserId} weren't found!", supportTicketName, userId);
                    return (null, null);
                }

                var newMessage = new ChatMessage
                {
                    Text = message,
                    SenderId = user.UserId,
                    ReceiverId = supportTicket.Id,
                    CreatedAt = DateTime.UtcNow,
                };

                _logger.LogInformation("Adding custom message to mongo database!");

                await _publisher.PublishAsync(newMessage);

                _logger.LogInformation("Finished adding custom message to mongo database!");

                await _mysqluow.CommitAsync();

                _logger.LogInformation("Ended sending message from user with id: {UserId} to support ticket with name: {SupportTicketName}, Text: {Text}, Success!", userId, supportTicketName, message);

                return (newMessage, user);
            }
            catch (Exception ex)
            {
                await _mysqluow.RollbackAsync();
                _logger.LogError(ex, "Error, Something went wrong while sending message to support ticket with name: {SupportTicketName}", supportTicketName);
                throw;
            }
        }
    }
}
