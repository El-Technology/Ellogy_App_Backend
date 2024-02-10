using AutoMapper;
using PaymentManager.BLL.Models;
using PaymentManager.Common.Constants;
using Stripe;

namespace PaymentManager.BLL.Mapping;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductModel>()
            .ForMember(dest => dest.Name, opt =>
                opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt =>
                opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ProductId, opt =>
                opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.PriceId, opt =>
                opt.MapFrom(src => src.DefaultPrice != null
                    ? src.DefaultPrice.Id
                    : null))
            .ForMember(dest => dest.Price, opt =>
                opt.MapFrom(src => src.DefaultPrice != null
                    ? src.DefaultPrice.UnitAmountDecimal / Constants.PriceInCents
                    : null))
            .ForMember(dest => dest.Features, opt =>
                opt.MapFrom(src => src.Features.Select(f => f.Name)));
    }
}