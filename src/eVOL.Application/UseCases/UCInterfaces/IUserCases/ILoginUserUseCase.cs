using eVOL.Application.DTOs;
using eVOL.Application.DTOs.Responses;
using eVOL.Application.DTOs.Responses.User;
using eVOL.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.UCInterfaces.IUserCases
{
    public interface ILoginUserUseCase
    {
        Task<LoginUserResponse?> ExecuteAsync(LoginDTO dto);
    }
}
