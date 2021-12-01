using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Validation
{
    public class MainPageModelValidator : AbstractValidator<MainPageModel>
    {
        public MainPageModelValidator()
        {
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
