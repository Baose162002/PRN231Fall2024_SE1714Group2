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
            CreateMap<Batch, UpdateBatchDTO>();
            CreateMap<UpdateBatchDTO, Batch>();
            CreateMap<ListBatchDTO, Batch>();

            CreateMap<Batch, ListBatchDTO>();
               

            // Flower mappings
            CreateMap<CreateFlowerDTO, Flower>();
            CreateMap<UpdateFlowerDTO, Flower>();

            CreateMap<Order, CreateOrderDTO>();
            CreateMap<Order, UpdateOrderDTO>()
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString()));
            CreateMap<UpdateOrderDTO, Order>()
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString()))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateTime.ParseExact(src.OrderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)));
            CreateMap<ListOrderDTO, Order>()
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString()));
            CreateMap<Order, ListOrderDTO>()
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString()))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => FormatDate(src.OrderDate)))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer)) // Assuming you map customer to ListUserDTO
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails)); // Map OrderDetails to List<OrderDetailDTO
                
            CreateMap<Flower, ListFlowerDTO>();
            CreateMap<ListFlowerDTO, Flower>();

            // User mappings
            CreateMap<CreateUserDTO, User>();
            CreateMap<UpdateUserDTO, User>();
            CreateMap<User, ListUserDTO>();

            CreateMap<User, UserResponseDto>()
/*                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))  
*/                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));  // Convert enum Status to string


            // Company mappings
            CreateMap<Company, CompanyDTO>();
            CreateMap<CompanyDTO, Company>();

            CreateMap<OrderDetail, OrderDetailDTO>();
            CreateMap<OrderDetailDTO, OrderDetail>();
            //CreateMap<UpdateOrderDetailDTO, Order>()
            //    .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString()))
            //    .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateTime.ParseExact(src.OrderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)));
            CreateMap<ListOrderDetailDTO, Order>();
            CreateMap<Order, ListOrderDetailDTO>();


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
