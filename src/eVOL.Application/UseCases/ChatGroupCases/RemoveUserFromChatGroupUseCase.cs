using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.ChatGroupCases
{
    public class RemoveUserFromChatGroupUseCase
    {
        private readonly IMySqlUnitOfWork _uow;

        public RemoveUserFromChatGroupUseCase(IMySqlUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<User?> ExecuteAsync(int userId, int chatGroupId)
        {

            await _uow.BeginTransactionAsync();

            try
            {
                var user = await _uow.Users.GetUserById(userId);

                var chatGroup = await _uow.ChatGroup.GetChatGroupById(chatGroupId);

                if (user == null || chatGroup == null || chatGroup.GroupUsers.Contains(user))
                {
                    return null;
                }

                chatGroup.GroupUsers.Remove(user);
                chatGroup.TotalUsers -= 1;

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
