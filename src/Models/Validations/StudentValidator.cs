using DapperUniversity.Models;
using FluentValidation;

namespace DapperUniversity.Models.Validators
{
    public class StudentValidator : AbstractValidator<Student>
    {
        public StudentValidator()
        {
            RuleFor(x => x.FirstName)
                .Length(1, 50)
                .WithMessage("Please limit first name to 50 characters.");
            RuleFor(x => x.LastName)
                .Length(1, 50)
                .WithMessage("Please limit last name to 50 characters.");
        }
    }
}
