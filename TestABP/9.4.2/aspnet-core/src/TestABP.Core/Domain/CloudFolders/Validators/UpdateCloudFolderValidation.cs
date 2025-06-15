using FluentValidation;
using TestABP.Domain.CloudFolders.Dto;
using TestABP.Validation;

namespace TestABP.Domain.CloudFolders.Validators
{
    public class UpdateCloudFolderValidation : AbstractValidator<UpdateCloudFolderDto>
    {
        public UpdateCloudFolderValidation()
        {

            RuleFor(x => x.Name)
            .Must(n => !string.IsNullOrEmpty(n))
            .WithMessage(ValidationMessage.IsNotNullOrEmpty(nameof(UpdateCloudFolderDto.Name)));

            RuleFor(x => x.Code)
            .Must(c => !string.IsNullOrWhiteSpace(c))
            .WithMessage(ValidationMessage.IsNotNullOrWhiteSpace(nameof(UpdateCloudFolderDto.Code)));
        }
    }
}
