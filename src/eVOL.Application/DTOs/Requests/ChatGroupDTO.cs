using eVOL.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.DTOs.Requests
{
    public class ChatGroupDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public int TotalUsers { get; set; }
        [Required]
        public List<User>? GroupUsers { get; set; }
        [Required]
        public int OwnerId { get; set; }
    }
}
