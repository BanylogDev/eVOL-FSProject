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
    public class TransferOwnershipOfChatGroupUseCase : ITransferOwnershipOfChatGroupUseCase
    {
        private readonly IMySqlUnitOfWork _uow;
        private readonly ILogger<TransferOwnershipOfChatGroupUseCase> _logger;

        public TransferOwnershipOfChatGroupUseCase(IMySqlUnitOfWork uow, ILogger<TransferOwnershipOfChatGroupUseCase> logger)
        {
            _uow = uow;
            _logger = logger;
        }   

        public async Task<ChatGroup?> ExecuteAsync(int currentOwnerId, int newOwnerId, int chatGroupId)
        {

            _logger.LogInformation("Started transfering chat group ownership with id: {ChatGroupId} from user with id: {CurrentOwnerUserId} to user with id: {NewOwnerUserId}", chatGroupId, currentOwnerId, newOwnerId);

            await _uow.BeginTransactionAsync();

            try
            {
                var currentOwner = await _uow.Users.GetUserById(currentOwnerId);

                var newOwner = await _uow.Users.GetUserById(newOwnerId);

                var chatGroup = await _uow.ChatGroup.GetChatGroupById(chatGroupId);

                if (currentOwner == null || newOwner == null || chatGroup == null || chatGroup.OwnerId != currentOwnerId)
                {
                    _logger.LogWarning("Current chat group owner with id: {CurrentOwnerUserId} or New chat group owner with id: {NewOwnerUserId} or chat group with id: {ChatGroupId} weren't found or user trying to transfer ownership isn't the actual owner", currentOwnerId, newOwnerId, chatGroupId);
                    return null;
                }

                _logger.LogInformation("Transfering chat group ownership with id: {ChatGroupId} from user with id: {CurrentOwnerUserId} to user with id: {NewOwnerUserId}", chatGroupId, currentOwnerId, newOwnerId);

                chatGroup.OwnerId = newOwnerId;

                _logger.LogInformation("Finished transfering chat group ownership with id: {ChatGroupId} from user with id: {CurrentOwnerUserId} to user with id: {NewOwnerUserId}", chatGroupId, currentOwnerId, newOwnerId);

                await _uow.CommitAsync();

                _logger.LogInformation("Ended transfering chat group ownership with id: {ChatGroupId} from user with id: {CurrentOwnerUserId} to user with id: {NewOwnerUserId}, Success!", chatGroupId, currentOwnerId, newOwnerId);

                return chatGroup;
            }
            catch (Exception ex) 
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "Error, Something went wrong while transfering the chat group ownership with id: {ChatGroupId} from user with id: {CurrentOwnerUserId} to user with id: {NewOwnerUserId}", chatGroupId, currentOwnerId, newOwnerId);
                throw;
            }


        }
    }
}
