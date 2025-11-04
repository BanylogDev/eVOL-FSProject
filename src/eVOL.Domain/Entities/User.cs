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
        [Required]
        [StringLength(16, MinimumLength = 4)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        public Address? Address { get; set; }
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; }


        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
