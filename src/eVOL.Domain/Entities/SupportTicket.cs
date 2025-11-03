using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Domain.Entities
{
    public class SupportTicket
    {
        public int Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public int OpenedBy { get; set; }
        public int ClaimedBy { get; set; }
        public bool ClaimedStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
