using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.DTOs.Responses.User
{
    public class RegisterUserResponse : BaseUserResponse
    {   
        public DateTime CreatedAt { get; set; }
    }
}
