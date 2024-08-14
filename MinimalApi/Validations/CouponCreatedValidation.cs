using FluentValidation;
using MinimalApi.Data.Dto;

namespace MinimalApi.Validations {
    public class CouponCreatedValidation : AbstractValidator<CouponCreatedDto>{
        public CouponCreatedValidation()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Percent).InclusiveBetween(1, 100);
        }
    }
}
