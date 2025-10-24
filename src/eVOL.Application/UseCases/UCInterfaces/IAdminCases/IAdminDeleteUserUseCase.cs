using eVOL.Application.DTOs;
using eVOL.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.UCInterfaces.IAdminCases
{
    public interface IAdminDeleteUserUseCase
    {
        Task<User?> ExecuteAsync(int id);
    }
}
