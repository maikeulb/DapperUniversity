using DapperUniversity.Models;
using FluentValidation;

namespace DapperUniversity.Models.Validators
{
    public class DepartmentValidator : AbstractValidator<Department>
    {
        public DepartmentValidator()
        {
            RuleFor(x => x.Name)
                .Length(1, 50)
                .WithMessage("Please limit name to 50 characters.");
        }
    }
}
