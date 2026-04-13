using FluentValidation;

namespace Noticore.Application.Notifications.Commands.CreateNotification
{
    public class CreateNotificationValidator : AbstractValidator<CreateNotificationCommand>
    {
        public CreateNotificationValidator()
        {
            RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

            RuleFor(x => x.Recipient)
                .NotEmpty().WithMessage("Recipient is required.");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message content is required.");
        }
    }
}
