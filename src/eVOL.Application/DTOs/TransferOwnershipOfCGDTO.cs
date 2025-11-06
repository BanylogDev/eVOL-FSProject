using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.DTOs
{
    public class TransferOwnershipOfCGDTO
    {
        [Required]
        public int ChatGroupId { get; set; }
        [Required]
        public int CurrentOwnerId { get; set; }
        [Required]
        public int NewOwnerId { get; set; }
    }
}
