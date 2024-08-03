using FluentValidation;
using LXP.Common.ViewModels.QuizQuestionViewModel;
using LXP.Common.Constants;

namespace LXP.Common.Validators
{
    public class QuizQuestionViewModelValidator : AbstractValidator<QuizQuestionViewModel>
    {
        public QuizQuestionViewModelValidator()
        {
            RuleFor(question => question.Question)
                .NotEmpty()
                .WithMessage("Question text is required.");

            RuleFor(question => question.QuestionType)
                .NotEmpty()
                .WithMessage("Question type is required.")
                .Must(BeAValidQuestionType)
                .WithMessage("Invalid question type.");

            RuleFor(question => question.Options)
                .NotEmpty()
                .WithMessage("Options are required.")
                .Must((model, options) => ValidateOptionsByQuestionType(model.QuestionType, options))
                .WithMessage("Invalid options for the given question type.");
        }

        private bool BeAValidQuestionType(string questionType)
        {
            return QuizQuestionTypes.MultiSelectQuestion.Equals(questionType, StringComparison.OrdinalIgnoreCase)
                || QuizQuestionTypes.MultiChoiceQuestion.Equals(questionType, StringComparison.OrdinalIgnoreCase)
                || QuizQuestionTypes.TrueFalseQuestion.Equals(questionType, StringComparison.OrdinalIgnoreCase);
        }

        private bool ValidateOptionsByQuestionType(string questionType, List<QuestionOptionViewModel> options)
        {
            switch (questionType.ToUpper())
            {
                case QuizQuestionTypes.MultiSelectQuestion:
                    return options.Count >= 5
                        && options.Count <= 8
                        && options.Count(o => o.IsCorrect) >= 2
                        && options.Count(o => o.IsCorrect) <= 3;
                case QuizQuestionTypes.MultiChoiceQuestion:
                    return options.Count == 4 && options.Count(o => o.IsCorrect) == 1;
                case QuizQuestionTypes.TrueFalseQuestion:
                    return options.Count == 2
                        && options.Count(o => o.IsCorrect) == 1
                        && options.Any(o => o.Option.ToLower() == "true")
                        && options.Any(o => o.Option.ToLower() == "false");
                default:
                    return false;
            }
        }
    }
}
