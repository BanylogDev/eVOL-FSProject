using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Domain.Entities
{
    public class ChatGroup
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TotalUsers { get; set; }
        public List<User> GroupUsers { get; set; }
        public int OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
