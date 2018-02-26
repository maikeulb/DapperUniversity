using DapperUniversity.Models;
using FluentValidation;

namespace DapperUniversity.Models.Validators
{
    public class CourseValidator : AbstractValidator<Course>
    {
        public CourseValidator()
        {
            RuleFor(x => x.Title)
                .Length(1, 50)
                .WithMessage("Please limit title to 50 characters.");
        }
    }
}
