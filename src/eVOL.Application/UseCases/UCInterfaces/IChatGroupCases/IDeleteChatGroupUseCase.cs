using eVOL.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.UCInterfaces.IChatGroupCases
{
    public interface IDeleteChatGroupUseCase
    {
        Task<ChatGroup?> ExecuteAsync(int chatGroupId, int chatGroupOwnerId);
    }
}
