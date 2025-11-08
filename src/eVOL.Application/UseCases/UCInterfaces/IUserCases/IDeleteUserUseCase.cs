using eVOL.Application.DTOs.Requests;
using eVOL.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.UCInterfaces.IUserCases
{
    public interface IDeleteUserUseCase
    {
        Task<User?> ExecuteAsync(DeleteAccountDTO dto);
    }
}
