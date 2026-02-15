using FluentValidation;

namespace Lerevently.Modules.Events.Application.Events.CreateEvent;

internal sealed class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.Location)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.StartsAtUtc)
            .NotEmpty()
            .GreaterThan(DateTime.UtcNow);

        RuleFor(x => x.EndsAtUtc)
            .GreaterThan(x => x.StartsAtUtc).When(x => x.EndsAtUtc.HasValue);
    }
}