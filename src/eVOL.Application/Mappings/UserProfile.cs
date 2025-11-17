using Mapster;
using eVOL.Application.DTOs.Responses;
using eVOL.Application.DTOs.Responses.User;
using eVOL.Domain.Entities;

namespace eVOL.Application.Mappings
{
    public class UserMappings : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<User, LoginUserResponse>();
            config.ForType<User, RegisterUserResponse>();
            config.ForType<User, GetUserResponse>();
        }
    }
}
