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
    public class LeaveChatGroupUseCase : ILeaveChatGroupUseCase
    {
        private readonly IMySqlUnitOfWork _uow;
        private readonly ILogger<LeaveChatGroupUseCase> _logger;

        public LeaveChatGroupUseCase(IMySqlUnitOfWork uow, ILogger<LeaveChatGroupUseCase> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<User?> ExecuteAsync(int userId, string chatGroupName)
        {

            _logger.LogInformation("Started removing user with id: {UserId} from chat group with name {ChatGroupName}", userId, chatGroupName);

            await _uow.BeginTransactionAsync();

            try
            {
                var user = await _uow.Users.GetUserById(userId);

                var chatGroup = await _uow.ChatGroup.GetChatGroupByName(chatGroupName);

                if (chatGroup == null || user == null || !chatGroup.GroupUsers.Contains(user))
                {
                    _logger.LogWarning("Chat group with name: {ChatGroupName} or user with id: {UserId} wasn't found, or user isn't in the group!", chatGroupName, userId);
                    return null;
                }

                _logger.LogInformation("Removing user with id: {UserId} from chat group with name {ChatGroupName}, Previous Total Users: {TotalUsers}", userId, chatGroupName, chatGroup.TotalUsers);

                chatGroup.GroupUsers.Remove(user);
                chatGroup.TotalUsers -= 1;

                _logger.LogInformation("Finished removing user with id: {UserId} from chat group with name {ChatGroupName}, New Total Users: {TotalUsers}", userId, chatGroupName, chatGroup.TotalUsers);

                if (chatGroup.TotalUsers == 0)
                {
                    _uow.ChatGroup.DeleteChatGroup(chatGroup);
                    _logger.LogInformation("Chat Group with name: {ChatGroupName} has been deleted because all users in it left!", chatGroupName);
                }

                await _uow.CommitAsync();

                _logger.LogInformation("Ended removing user with id: {UserId} from chat group with name {ChatGroupName}, Success!", userId, chatGroupName);

                return user;
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogInformation(ex, "Error, Something went wrong during the subtraction of user with id: {UserId} from chat group with name: {ChatGroupName}", userId, chatGroupName);
                throw;
            }


        }
    }
}
