using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SipkaTemplate.Core.DTOs.HelperDTOs;
using SipkaTemplate.Core.Enums;

namespace SipkaTemplate.Core.DTOs
{
    public class UserDto:BaseDto
    {
        public string Name { get; set; }
        public string? ContentUrl { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
    }
}
