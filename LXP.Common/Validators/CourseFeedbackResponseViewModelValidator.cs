using FluentValidation;
using LXP.Common.ViewModels.FeedbackResponseViewModel;

namespace LXP.Common.Validators
{
    public class CourseFeedbackResponseViewModelValidator
        : AbstractValidator<CourseFeedbackResponseViewModel>
    {
        public CourseFeedbackResponseViewModelValidator()
        {
            RuleFor(feedback => feedback.CourseFeedbackQuestionId)
                .NotEmpty()
                .WithMessage("CourseFeedbackQuestionId is required.");

            RuleFor(feedback => feedback.LearnerId)
                .NotEmpty()
                .WithMessage("LearnerId is required.");

            RuleFor(feedback => feedback.Response)
                .NotEmpty()
                .When(feedback => string.IsNullOrEmpty(feedback.OptionText))
                .WithMessage("Either Response or OptionText must be provided.");

            RuleFor(feedback => feedback.OptionText)
                .NotEmpty()
                .When(feedback => string.IsNullOrEmpty(feedback.Response))
                .WithMessage("Either Response or OptionText must be provided.");
        }
    }
}
