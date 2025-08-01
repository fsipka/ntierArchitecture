using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SipkaTemplate.Core.DTOs.HelperDTOs;

namespace SipkaTemplate.Service.Validations
{
    public class ForgotPasswordDtoValidator:AbstractValidator<ForgotPasswordDto>
    {
        public ForgotPasswordDtoValidator()
        {
                    RuleFor(x => x.NewPassword)
             .MinimumLength(8)
             .WithMessage("New password must be at least 8 characters long and contain at least one letter and one number.")
             .When(x => !string.IsNullOrEmpty(x.NewPassword))
             .Matches(@"^(?=.*[A-Za-z])(?=.*\d).+$")
             .WithMessage("New password must be contain at least one letter and one number.");

        }
    }
}
