using AutoMapper;
using MinimalApi.Data.Dto;

namespace MinimalApi {
    public class MappingConfig : Profile {
        public MappingConfig()
        {
            CreateMap<Coupon, CouponCreatedDto>().ReverseMap();
            CreateMap<Coupon, CouponDto>().ReverseMap();
            CreateMap<Coupon, CouponUpdatedDto>().ReverseMap();
        }
    }
}
