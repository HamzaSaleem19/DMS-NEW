using FluentValidation;
using DMS.Models;

namespace DMS.Validators;

public class DocumentValidator : AbstractValidator<Document>
{
    public DocumentValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Document title is required")
            .MaximumLength(255).WithMessage("Title cannot exceed 255 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required")
            .MaximumLength(255).WithMessage("File name cannot exceed 255 characters");

        RuleFor(x => x.FilePath)
            .NotEmpty().WithMessage("File path is required");

        RuleFor(x => x.FileSize)
            .GreaterThan(0).WithMessage("File size must be greater than 0")
            .LessThanOrEqualTo(104857600).WithMessage("File size cannot exceed 100MB");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Please select a category");

        RuleFor(x => x.UploadedById)
            .NotEmpty().WithMessage("Uploaded by user ID is required");
    }
}
