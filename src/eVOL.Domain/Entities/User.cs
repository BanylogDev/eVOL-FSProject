using eVOL.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Address? Address { get; set; }
        public string Role { get; set; } = "User";
        public Money? Money { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<ChatGroup> ChatGroups { get; set; } = new List<ChatGroup>();
        public ICollection<SupportTicket> SupportTickets { get; set; } = new List<SupportTicket>();

        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
