using eVOL.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.UCInterfaces.IChatGroupCases
{
    public interface ILeaveChatGroupUseCase
    {
        Task<User?> ExecuteAsync(int userId, string chatGroupName);
    }
}
