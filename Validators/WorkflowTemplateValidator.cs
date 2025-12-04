using FluentValidation;
using DMS.Models;

namespace DMS.Validators;

public class WorkflowTemplateValidator : AbstractValidator<WorkflowTemplate>
{
    public WorkflowTemplateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Workflow template name is required")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
    }
}

public class WorkflowStepValidator : AbstractValidator<WorkflowStep>
{
    public WorkflowStepValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Step name is required")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.StepOrder)
            .GreaterThan(0).WithMessage("Step order must be greater than 0");

        RuleFor(x => x.WorkflowTemplateId)
            .GreaterThan(0).WithMessage("Workflow template ID is required");
    }
}
