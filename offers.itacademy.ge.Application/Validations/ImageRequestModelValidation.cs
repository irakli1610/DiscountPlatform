using FluentValidation;
using Microsoft.Extensions.Options;
using offers.itacademy.ge.Application.Models.Images;
using offers.itacademy.ge.Application.Services.ImageServices;
using offers.itacademy.ge.Application.Validations.Resources;

namespace offers.itacademy.ge.Application.Validations
{
    public class ImageRequestModelValidation : AbstractValidator<ImageRequestModel>
    {
        private readonly IOptions<ImageUploadSettings> _options;
        public ImageRequestModelValidation(IOptions<ImageUploadSettings> options) 
        {
            _options = options;

             RuleFor(x => x.File)
                .NotNull().WithMessage(ValidationMessages.FileIsRequired)
                .Must(file => file!.Length > 0).WithMessage(ValidationMessages.FileCanNotBeEmpty)
                .Must(file => _options.Value.AllowedExtensions.Contains(Path.GetExtension(file!.FileName).ToLower()))
                .WithMessage($"{ValidationMessages.OnlyFollowingAllowedTypes}: {string.Join(", ", _options.Value.AllowedExtensions)}")
                .Must(file => file!.Length <= _options.Value.MaxFileSize)
                .WithMessage($"{ValidationMessages.MaxFileSize} {(_options.Value.MaxFileSize / (1024 * 1024))}MB.");
        
        }
    }
}
