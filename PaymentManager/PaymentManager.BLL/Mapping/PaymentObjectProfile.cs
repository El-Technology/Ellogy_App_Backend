using AutoMapper;
using PaymentManager.BLL.Models;
using PaymentManager.Common.Constants;

namespace PaymentManager.BLL.Mapping
{
    public class PaymentObjectProfile : Profile
    {
        public PaymentObjectProfile()
        {
            CreateMap<Stripe.PaymentIntent, PaymentObject>()
                .ForMember(dest => dest.Product, opt =>
                    opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Date, opt =>
                    opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.Amount, opt =>
                    opt.MapFrom(src => src.Amount / Constants.PriceInCents))
                .ForMember(dest => dest.Status, opt =>
                    opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.DownloadLink, opt =>
                    opt.MapFrom(src => src.Invoice != null ? src.Invoice.InvoicePdf : null));
        }
    }
}
