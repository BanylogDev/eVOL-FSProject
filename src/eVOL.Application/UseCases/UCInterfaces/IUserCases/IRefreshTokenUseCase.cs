﻿using eVOL.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.UCInterfaces.IUserCases
{
    public interface IRefreshTokenUseCase
    {
        Task<TokenDTO?> ExecuteAsync(TokenDTO dto);
    }
}
