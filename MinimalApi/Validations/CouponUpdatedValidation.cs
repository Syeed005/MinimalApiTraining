using FluentValidation;
using MinimalApi.Data.Dto;

namespace MinimalApi.Validations {
    public class CouponUpdatedValidation : AbstractValidator<CouponUpdatedDto>{
        public CouponUpdatedValidation()
        {
            //RuleFor(x => x.Id).NotEqual(0);
            RuleFor(x => x.Id).NotEmpty().GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Percent).InclusiveBetween(1, 100);
        }
    }
}
