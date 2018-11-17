using FluentValidation;

namespace ElronAPI.Application.TrainTime.Queries
{
    public class TrainTimesQueryValidator : AbstractValidator<TrainTimesQuery>
    {
        public TrainTimesQueryValidator()
        {
            RuleFor(x => x.Origin)
                .NotEqual(x => x.Destination)
                .NotEmpty();
            
            RuleFor(x => x.Destination)
                .NotEqual(x => x.Origin)
                .NotEmpty();
        }
    }
}