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
    public class GetChatGroupByIdUseCase : IGetChatGroupByIdUseCase
    {
        private readonly IMySqlUnitOfWork _uow;
        private readonly ILogger<GetChatGroupByIdUseCase> _logger;

        public GetChatGroupByIdUseCase(IMySqlUnitOfWork uow, ILogger<GetChatGroupByIdUseCase> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<ChatGroup?> ExecuteAsync(int id)
        {

            _logger.LogInformation("Started geting chat group with id: {ChatGroupId}", id);

            var chatGroup =  await _uow.ChatGroup.GetChatGroupById(id);

            if (chatGroup == null)
            {
                _logger.LogWarning("Chat group wasn't found with id: {ChatGroupId}", id);
                return null;
            }

            _logger.LogInformation("Ended getting chat group with id: {ChatGroupId}, Success!", id);

            return chatGroup;
        }
    }
}
