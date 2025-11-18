using eVOL.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.DTOs.Responses.User
{
    public sealed class GetUserResponse : BaseUserResponse
    {
        public Address Address { get; set; }
        public Money Money { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
