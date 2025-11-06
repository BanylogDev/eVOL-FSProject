using eVOL.Application.UseCases.UCInterfaces.IChatGroupCases;
using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
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

        public TransferOwnershipOfChatGroupUseCase(IMySqlUnitOfWork uow)
        {
            _uow = uow;
        }   

        public async Task<ChatGroup?> ExecuteAsync(int currentOwnerId, int newOwnerId, int chatGroupId)
        {

            await _uow.BeginTransactionAsync();

            try
            {
                var currentOwner = await _uow.Users.GetUserById(currentOwnerId);

                var newOwner = await _uow.Users.GetUserById(newOwnerId);

                var chatGroup = await _uow.ChatGroup.GetChatGroupById(chatGroupId);

                if (currentOwner == null || newOwner == null || chatGroup == null || chatGroup.OwnerId != currentOwnerId)
                {
                    return null;
                }

                chatGroup.OwnerId = newOwnerId;

                await _uow.CommitAsync();

                return chatGroup;
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }


        }
    }
}
