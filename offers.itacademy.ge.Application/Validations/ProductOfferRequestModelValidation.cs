using FluentValidation;
using offers.itacademy.ge.Application.Models.ProductOffers;
using offers.itacademy.ge.Application.Validations.Resources;

namespace offers.itacademy.ge.Application.Validations
{
    public class ProductOfferRequestModelValidation : AbstractValidator<ProductOfferRequestModel>
    {
        public ProductOfferRequestModelValidation() 
        {
            RuleFor(x=>x.Name).NotEmpty().WithMessage(ValidationMessages.NameRequired)
                .MaximumLength(100).WithMessage(ValidationMessages.MaximumLengthForName);

            RuleFor(x => x.Description).NotEmpty().WithMessage(ValidationMessages.DescriptionRequired)
               .MaximumLength(500).WithMessage(ValidationMessages.MaximumLengthForDescription);

            RuleFor(x => x.Price).NotEmpty().WithMessage(ValidationMessages.PriceRequired)
                .GreaterThanOrEqualTo(0).WithMessage(ValidationMessages.PriceGreaterThanOrEqualZero);

            RuleFor(x=>x.Quantity).NotEmpty().WithMessage(ValidationMessages.QuantityRequired)
                .GreaterThanOrEqualTo(0).WithMessage(ValidationMessages.QuantityGreaterThanOrEqualZero);

            RuleFor(x=>x.ExpirationTime).NotEmpty().WithMessage(ValidationMessages.ExpirationTimerequired)
                .GreaterThan(DateTime.UtcNow.AddHours(1)).WithMessage(ValidationMessages.ExpirationTimeGreaterThanUtcNowPlusOneHour);

            RuleFor(x => x.CategoryId).NotEmpty().WithMessage(ValidationMessages.CategoryIdRequired);

        }
    }
}
