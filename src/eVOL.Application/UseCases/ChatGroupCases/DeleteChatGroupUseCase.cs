using eVOL.Application.DTOs;
using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.ChatGroupCases
{
    public class DeleteChatGroupUseCase
    {
        private readonly IMySqlUnitOfWork _uow;

        public DeleteChatGroupUseCase(IMySqlUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ChatGroup?> ExecuteAsync(int id)
        {

            await _uow.BeginTransactionAsync();

            try
            {
                var chatGroup = await _uow.ChatGroup.GetChatGroupById(id);

                if (chatGroup == null)
                {
                    return null;
                }

                await _uow.ChatGroup.CreateChatGroup(chatGroup);
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
