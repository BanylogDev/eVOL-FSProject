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
    public class CreateChatGroupUseCase : ICreateChatGroupUseCase
    {
        private readonly IMySqlUnitOfWork _uow;
        private readonly ILogger<CreateChatGroupUseCase> _logger;

        public CreateChatGroupUseCase(IMySqlUnitOfWork uow, ILogger<CreateChatGroupUseCase> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<ChatGroup> ExecuteAsync(ChatGroupDTO dto)
        {

            _logger.LogInformation("Started creating chat group with name: {ChatGroupName}", dto.Name);

            await _uow.BeginTransactionAsync();

            try
            {

                var chatGroup = new ChatGroup
                {
                    Name = dto.Name,
                    TotalUsers = dto.TotalUsers,
                    GroupUsers = dto.GroupUsers,
                    OwnerId = dto.OwnerId,
                    CreatedAt = DateTime.UtcNow,
                };

                _logger.LogInformation("Creating chat group with name: {ChatGroupName}", dto.Name);

                await _uow.ChatGroup.CreateChatGroup(chatGroup);
                await _uow.CommitAsync();

                _logger.LogInformation("Finished creating chat group with name: {ChatGroupName}, Success!", dto.Name);

                return chatGroup;
            }
            catch (Exception ex) 
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "Error, Something went wrong during the creation of the chat group with name: {ChatGroupName}", dto.Name);
                throw;  
            }

        }
    }
}
