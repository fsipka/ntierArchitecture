﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SipkaTemplate.Core.DTOs.HelperDTOs
{
    public class ForgotPasswordDto
    {
        public string Email { get; set; }
        public string? NewPassword { get; set; }
        public string? Code { get; set; }
    }
}
