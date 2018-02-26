using DapperUniversity.Models;
using FluentValidation;

namespace DapperUniversity.Models.Validators
{
    public class CourseValidator : AbstractValidator<Course>
    {
        public CourseValidator()
        {
            RuleFor(x => x.Title)
                .Length(50)
                .WithMessage("Please limit title to 50 characters.");
            RuleFor(x => x.Credits)
                .GreaterThan(0)
                .WithMessage("Please keep the credits range between 1-5")
                .LessThan(6)
                .WithMessage("Please keep the credits range between 1-5");
        }
    }
}
