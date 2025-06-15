using FluentValidation;
using TestABP.Domain.CloudFolders.Dto;
using TestABP.Validation;

namespace TestABP.Domain.CloudFolders.Validators
{
    public class CreateCloudFolderValidation : AbstractValidator<CreateCloudFolderDto>
    {
        public CreateCloudFolderValidation()
        {
            RuleFor(x => x.Name)
            .Must(n => !string.IsNullOrEmpty(n))
            .WithMessage(ValidationMessage.IsNotNullOrEmpty(nameof(CreateCloudFolderDto.Name)));

            RuleFor(x => x.Code)
            .Must(c => !string.IsNullOrWhiteSpace(c))
            .WithMessage(ValidationMessage.IsNotNullOrWhiteSpace(nameof(CreateCloudFolderDto.Code)));
        }
    }
}
