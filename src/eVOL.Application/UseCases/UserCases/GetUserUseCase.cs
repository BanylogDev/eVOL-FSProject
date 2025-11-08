using AutoMapper;
using eVOL.Application.DTOs.Responses.User;
using eVOL.Application.UseCases.UCInterfaces.IUserCases;
using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.UserCases
{
    public class GetUserUseCase : IGetUserUseCase
    {
        private readonly IMySqlUnitOfWork _uow;
        private readonly ILogger<GetUserUseCase> _logger;
        private readonly IMapper _mapper;

        public GetUserUseCase(IMySqlUnitOfWork uow, ILogger<GetUserUseCase> logger, IMapper mapper)
        {
            _uow = uow;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<GetUserResponse?> ExecuteAsync(int id)
        {
            var user = await _uow.Users.GetUserById(id);

            if (user == null)
            {
                _logger.LogWarning("GetUserUseCase: User with ID {UserId} not found.", id);
                return null;
            }

            _logger.LogInformation("GetUserUseCase: Successfully retrieved User with ID {UserId}.", id);

            return _mapper.Map<GetUserResponse>(user);   
        }
    }
}
