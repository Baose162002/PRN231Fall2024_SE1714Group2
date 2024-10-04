using AutoMapper;
using BusinessObject;
using BusinessObject.Dto.Request;
using BusinessObject.Dto.Response;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using BusinessObject.Enum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Configurations.Mapper
{
    public class MapperEntities : Profile
    {
        public MapperEntities()
        {
            // Batch mappings
            CreateMap<Batch, CreateBatchDTO>();
            CreateMap<Batch, UpdateBatchDTO>()
                .ForMember(dest => dest.BatchStatus, opt => opt.MapFrom(src => src.BatchStatus.ToString()));
            CreateMap<UpdateBatchDTO, Batch>()
               .ForMember(dest => dest.BatchStatus, opt => opt.MapFrom(src => src.BatchStatus.ToString()))
               .ForMember(dest => dest.EntryDate, opt => opt.MapFrom(src => DateTime.ParseExact(src.EntryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)));
            CreateMap<ListBatchDTO, Batch>()
                .ForMember(dest => dest.BatchStatus, opt => opt.MapFrom(src => src.BatchStatus.ToString()));

            CreateMap<Batch, ListBatchDTO>()
                  .ForMember(dest => dest.BatchStatus, opt => opt.MapFrom(src => src.BatchStatus.ToString()))
                  .ForMember(dest => dest.EntryDate,
                  opt => opt.MapFrom(src => FormatDate(src.EntryDate)));

            // Flower mappings
            CreateMap<CreateFlowerDTO, Flower>();
            CreateMap<UpdateFlowerDTO, Flower>();

            CreateMap<CreateOrderDTO, Order>();
            CreateMap<UpdateOrderDTO, Order>()
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString()))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateTime.ParseExact(src.OrderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)));
            CreateMap<ListOrderDTO, Order>()
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString()));
            CreateMap<Order, ListOrderDTO>()
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString()))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => FormatDate(src.OrderDate)));
                
            CreateMap<Flower, ListFlowerDTO>();
            CreateMap<ListFlowerDTO, Flower>();

            // User mappings
            CreateMap<CreateUserDTO, User>();
            CreateMap<UpdateUserDTO, User>();
            CreateMap<User, ListUserDTO>();
            CreateMap<User, UserResponseDto>(); // Add this mapping

            // Company mappings
            CreateMap<Company, CompanyDTO>();
            CreateMap<CompanyDTO, Company>();

            CreateMap<Flower, ListFlowerDTO>();
            CreateMap<ListFlowerDTO, Flower>();
            CreateMap<LoginUserRequest, User>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));
            // Review mappings
            CreateMap<CreateReviewDTO, Review>();
            CreateMap<UpdateReviewDTO, Review>();
            CreateMap<Review, ListReviewDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName));

            // Login mapping
            CreateMap<LoginUserRequest, User>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));
        }

        private string FormatDate(DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("dd/MM/yyyy") : null;
        }
    }
}
