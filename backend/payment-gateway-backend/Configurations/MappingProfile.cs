using AutoMapper;
using payment_gateway_backend.Entities;
using payment_gateway_backend.Models.EventLog;
using payment_gateway_backend.Models.Payment;
using payment_gateway_backend.Models.PaymentTransaction;

namespace payment_gateway_backend.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PaymentRequestDto, PaymentTransaction>();

            CreateMap<PaymentTransaction, PaymentResponseDto>()
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => GetPaymentMessage(src.Status)));

            CreateMap<PaymentTransaction, PaymentTransactionDto>();
            CreateMap<CreatePaymentTransactionDto, PaymentTransaction>();

            CreateMap<EventLog, EventLogDto>()
                .ForMember(dest => dest.EventData, opt => opt.MapFrom(src => src.Payload));
            CreateMap<CreateEventLogDto, EventLog>();
        }

        private static string GetPaymentMessage(string status)
        {
            return status switch
            {
                "Success" => "Payment processed successfully",
                "Pending" => "Payment is under review",
                _ => "Payment failed due to insufficient funds"
            };
        }
    }
}
