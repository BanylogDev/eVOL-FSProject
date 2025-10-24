using eVOL.Application.DTOs;
using eVOL.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.UCInterfaces.IUserCases
{
    public interface IUpdateUserUseCase
    {
        Task<User?> ExecuteAsync(UpdateDTO dto);
    }
}
