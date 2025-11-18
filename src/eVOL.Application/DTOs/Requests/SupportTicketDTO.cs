using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.DTOs.Requests
{
    public sealed class SupportTicketDTO
    {
        [Required]
        public string Category { get; set; } = string.Empty;
        [Required]
        public string Text { get; set; } = string.Empty;
        [Required]
        public int OpenedBy { get; set; }
    }
}
