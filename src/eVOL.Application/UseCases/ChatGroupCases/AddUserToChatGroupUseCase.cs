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
    public class AddUserToChatGroupUseCase : IAddUserToChatGroupUseCase
    {
        private readonly IMySqlUnitOfWork _uow;
        private readonly ILogger<AddUserToChatGroupUseCase> _logger;

        public AddUserToChatGroupUseCase(IMySqlUnitOfWork uow, ILogger<AddUserToChatGroupUseCase> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<User?> ExecuteAsync(int userId, string chatGroupName)
        {

            _logger.LogInformation("Started adding user with id: {UserId} in chat group with name: {ChatGroupName}", userId, chatGroupName);

            await _uow.BeginTransactionAsync(); 

            try
            {
                var user = await _uow.Users.GetUserById(userId);

                var chatGroup = await _uow.ChatGroup.GetChatGroupByName(chatGroupName);

                if (user == null || chatGroup == null || chatGroup.GroupUsers.Contains(user))
                {
                    _logger.LogWarning("User with id: {UserId} or ChatGroup with name: {ChatGroupName} wasn't found or User is already in the group!", userId, chatGroupName);
                    return null;
                }

                _logger.LogInformation("Adding User with id: {UserId} in the chat group with name: {ChatGroupName}, Previous Total Users: {TotalUsers}", userId, chatGroupName, chatGroup.TotalUsers);

                chatGroup.GroupUsers.Add(user);
                chatGroup.TotalUsers += 1;

                _logger.LogInformation("Finished Adding User with id: {UserId} in the chat group with name: {ChatGroupName}, New Total Users: {TotalUsers}", userId, chatGroupName, chatGroup.TotalUsers);

                await _uow.CommitAsync();

                _logger.LogInformation("Ended adding user with id: {UserId} in chat group with name: {ChatGroupName}, Success!", userId, chatGroupName);

                return user;
            }
            catch (Exception ex) 
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "Error, Something went wrong during the addition of the user with id: {UserId} in the group with name: {ChatGroupName}, Failure!", userId, chatGroupName);
                throw;
            }
        }
    }
}
