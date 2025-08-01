using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SipkaTemplate.Core.DTOs.HelperDTOs;

namespace SipkaTemplate.Service.Validations
{
    public class ResetPasswordDtoValidator:AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
            .NotNull()
                .WithMessage("Email is required.");
            RuleFor(x => x.OldPassword)
                .NotEmpty()
                .WithMessage("Old password is required.")
                .NotNull()
                .WithMessage("Old password is required.");
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("New password is required.")
                    .NotNull()
                .WithMessage("New password is required.")
                .MinimumLength(8)
                .WithMessage("New password must be at least 8 characters long and contain at least one letter and one number.")
                .Matches(@"^(?=.*[A-Za-z])(?=.*\d).+$")
                .WithMessage("New password must be contain at least one letter and one number.");

        }
    }
}
