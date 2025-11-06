using eVOL.Application.DTOs;
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
    public class DeleteChatGroupUseCase : IDeleteChatGroupUseCase
    {
        private readonly IMySqlUnitOfWork _uow;

        public DeleteChatGroupUseCase(IMySqlUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ChatGroup?> ExecuteAsync(int chatGroupId, int chatGroupOwnerId)
        {

            await _uow.BeginTransactionAsync();

            try
            {
                var chatGroup = await _uow.ChatGroup.GetChatGroupById(chatGroupId);

                if (chatGroup == null || chatGroup.OwnerId != chatGroupOwnerId)
                {
                    return null;
                }

                _uow.ChatGroup.DeleteChatGroup(chatGroup);
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
