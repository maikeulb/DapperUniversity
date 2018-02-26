using DapperUniversity.Models;
using FluentValidation;

namespace DapperUniversity.Models.Validators
{
    public class OfficeAssignmentValidator : AbstractValidator<OfficeAssignment>
    {
        public OfficeAssignmentValidator()
        {
            RuleFor(x => x.Location)
                .Length(50)
                .WithMessage("Please limit location to 50 characters.");
        }
    }
}
