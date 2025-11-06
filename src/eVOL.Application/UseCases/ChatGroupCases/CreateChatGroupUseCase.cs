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
    public class CreateChatGroupUseCase : ICreateChatGroupUseCase
    {
        private readonly IMySqlUnitOfWork _uow;

        public CreateChatGroupUseCase(IMySqlUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ChatGroup> ExecuteAsync(ChatGroupDTO dto)
        {

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
