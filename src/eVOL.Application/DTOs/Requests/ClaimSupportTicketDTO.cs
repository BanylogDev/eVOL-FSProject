using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.DTOs.Requests
{
    public class ClaimSupportTicketDTO
    {
        [Required] 
        public int Id { get; set; }
        [Required]
        public int OpenedBy { get; set; }
    }
}
