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
        public string Name { get; set; }
        public string Category { get; set; }
        public string Text { get; set; }
        public int OpenedBy { get; set; }
        public int ClaimedBy { get; set; }
        public bool ClaimedStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
