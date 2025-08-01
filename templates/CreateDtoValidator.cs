using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Namespace.Core.DTOs;
using Namespace.Core.DTOs.CreateDTOs;

namespace Namespace.Service.Validations
{
    public class ModelNameCreateDtoValidator : AbstractValidator<ModelNameCreateDto>
    {
        public ModelNameCreateDtoValidator()
        {
            // RuleFor(x => x.Name).NotNull().WithMessage("Lütfen {PropertyName} kısmını boş bırakmayınız.").NotEmpty().WithMessage("{PropertyName} kısmı gereklidir.");

            // RuleFor(x => x.Email)
            //     .EmailAddress()
            //     .WithMessage("Lütfen geçerli bir e-posta adresi giriniz.");

            // RuleFor(x => x.Password)
            //  .MinimumLength(8)
            //  .WithMessage("New password must be at least 8 characters long and contain at least one letter and one number.")
            //  .When(x => !string.IsNullOrEmpty(x.Password))
            //  .Matches(@"^(?=.*[A-Za-z])(?=.*\d).+$")
            //  .WithMessage("New password must be contain at least one letter and one number.");
        }
    }
}
