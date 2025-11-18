using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Domain.Entities
{
    public class ChatMessage
    {
        public int MessageId { get; set; }
        public string Text { get; set; } = string.Empty;
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
