using AutoMapper;
using eVOL.Application.DTOs.Responses;
using eVOL.Application.DTOs.Responses.User;
using eVOL.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, LoginUserResponse>();
            CreateMap<User, RegisterUserResponse>();
            CreateMap<User, GetUserResponse>();
        }
    }
}
