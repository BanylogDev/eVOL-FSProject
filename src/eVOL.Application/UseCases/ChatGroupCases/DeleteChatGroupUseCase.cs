using eVOL.Application.DTOs;
using eVOL.Application.UseCases.UCInterfaces.IChatGroupCases;
using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.ChatGroupCases
{
    public class DeleteChatGroupUseCase : IDeleteChatGroupUseCase
    {
        private readonly IMySqlUnitOfWork _uow;
        private readonly ILogger<DeleteChatGroupUseCase> _logger;

        public DeleteChatGroupUseCase(IMySqlUnitOfWork uow, ILogger<DeleteChatGroupUseCase> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<ChatGroup?> ExecuteAsync(int chatGroupId, int chatGroupOwnerId)
        {

            _logger.LogInformation("Started deleting chat group with id: {ChatGroupId}", chatGroupId);

            await _uow.BeginTransactionAsync();

            try
            {
                var chatGroup = await _uow.ChatGroup.GetChatGroupById(chatGroupId);

                if (chatGroup == null || chatGroup.OwnerId != chatGroupOwnerId)
                {
                    _logger.LogWarning("Chat Group with id: {ChatGroupId} wasn't found or user that triggered the action with id: {UserId} isn't the owner of the chat group", chatGroupId, chatGroupOwnerId);
                    return null;
                }

                _logger.LogInformation("Deleting chat group with name: {ChatGroupName}", chatGroup.Name);

                _uow.ChatGroup.DeleteChatGroup(chatGroup);
                await _uow.CommitAsync();

                _logger.LogInformation("Ended deleting chat group with name: {ChatGroupName}, Success!", chatGroup.Name);

                return chatGroup;
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "Error, Something went wrong during the deletion of chat group with id: {ChatGroupId}", chatGroupId);
                throw;
            }

        }
    }
}
