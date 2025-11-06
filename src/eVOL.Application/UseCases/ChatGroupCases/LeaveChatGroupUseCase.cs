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
    public class LeaveChatGroupUseCase : ILeaveChatGroupUseCase
    {
        private readonly IMySqlUnitOfWork _uow;

        public LeaveChatGroupUseCase(IMySqlUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<User?> ExecuteAsync(int userId, string chatGroupName)
        {

            await _uow.BeginTransactionAsync();

            try
            {
                var user = await _uow.Users.GetUserById(userId);

                var chatGroup = await _uow.ChatGroup.GetChatGroupByName(chatGroupName);

                if (chatGroup == null || user == null || !chatGroup.GroupUsers.Contains(user))
                {
                    return null;
                }

                chatGroup.GroupUsers.Remove(user);
                chatGroup.TotalUsers -= 1;

                if (chatGroup.TotalUsers == 0)
                {
                    _uow.ChatGroup.DeleteChatGroup(chatGroup);
                }

                await _uow.CommitAsync();

                return user;
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }


        }
    }
}
