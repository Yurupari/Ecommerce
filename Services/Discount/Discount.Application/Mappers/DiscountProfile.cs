using AutoMapper;
using Discount.Application.Commands;
using Discount.Core.Entities;
using Discount.Grpc.Protos;

namespace Discount.Application.Mappers
{
    public class DiscountProfile : Profile
    {
        public DiscountProfile()
        {
            CreateMap<Coupon, CouponModel>().ReverseMap();
            CreateMap<CreateDiscountCommand, Coupon>().ReverseMap();
            CreateMap<UpdateDiscountCommand, Coupon>().ReverseMap();
        }
    }
}
