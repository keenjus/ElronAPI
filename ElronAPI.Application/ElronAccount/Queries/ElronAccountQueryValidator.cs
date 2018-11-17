using FluentValidation;

namespace ElronAPI.Application.ElronAccount.Queries
{
    public class ElronAccountQueryValidator : AbstractValidator<ElronAccountQuery>
    {
        public ElronAccountQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .MinimumLength(11)
                .MaximumLength(16);
        }
    }
}
