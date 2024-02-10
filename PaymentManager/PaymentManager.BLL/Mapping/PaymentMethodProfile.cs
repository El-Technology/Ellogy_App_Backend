using AutoMapper;
using Stripe;

namespace PaymentManager.BLL.Mapping;

public class PaymentMethodProfile : Profile
{
    public PaymentMethodProfile()
    {
        CreateMap<PaymentMethod, Models.PaymentMethod>()
            .ForMember(dest => dest.Type, opt =>
                opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Default, opt =>
                opt.MapFrom(src =>
                    (src.Customer.InvoiceSettings.DefaultPaymentMethodId ?? string.Empty).Equals(src.Id)));

        CreateMap<Card, Models.PaymentMethod>()
            .ForMember(dest => dest.CardBrand, opt =>
                opt.MapFrom(src => src.Brand))
            .ForMember(dest => dest.Expires, opt =>
                opt.MapFrom(src => $"{src.ExpMonth}/{src.ExpYear}"))
            .ForMember(dest => dest.Last4, opt =>
                opt.MapFrom(src => src.Last4));
    }
}