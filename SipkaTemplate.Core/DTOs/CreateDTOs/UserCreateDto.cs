using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SipkaTemplate.Core.DTOs.CreateDTOs
{
    public class UserCreateDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public string? TimeZone { get; set; }
        public string? Language { get; set; }
        public string? ContentUrl { get; set; }
        public string? GoogleId { get; set; }
    }
}
