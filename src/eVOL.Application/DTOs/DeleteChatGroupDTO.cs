using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.DTOs
{
    public class DeleteChatGroupDTO
    {
        [Required]
        public int ChatGroupId { get; set; }
        [Required]
        public int ChatGroupOwnerId { get; set; }
    }
}
