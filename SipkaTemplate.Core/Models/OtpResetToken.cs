using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SipkaTemplate.Core.Models
{
    public class OtpResetToken:BaseEntity
    {
        public string Email { get; set; }
        public string Code { get; set; } // örn: 6 haneli rakam
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
    }

}
