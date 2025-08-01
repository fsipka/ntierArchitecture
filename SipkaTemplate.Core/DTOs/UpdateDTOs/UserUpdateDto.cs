using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SipkaTemplate.Core.DTOs.UpdateDTOs
{
    public class UserUpdateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? TimeZone { get; set; }
        public string? Language { get; set; }

    }
}
