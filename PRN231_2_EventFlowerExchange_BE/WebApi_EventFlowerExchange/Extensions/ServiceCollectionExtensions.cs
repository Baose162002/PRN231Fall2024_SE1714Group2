
using BusinessObject.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Repository.IRepository;
using Repository.Repository;
using Service.Configurations.Mapper;
using Service.IService;
using Service.Service;
using System.Text.Json.Serialization;

namespace WebApi_EventFlowerExchange.Extensions
{

    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection Register(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });


            // Configure AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Register repositories here

            services.AddScoped<IBatchRepository, BatchRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            services.AddScoped<IFlowerRepository, FlowerRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IDeliveryRepository, DeliveryRepository>(); 
            // Register services here
            services.AddScoped<IBatchService, BatchService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderDetailService, OrderDetailService>();
            services.AddScoped<IFlowerService, FlowerService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IPayService, PaySerivce>();
            services.AddScoped<PaymentService>();
            services.AddScoped<VnPayLibrary>();


            services.AddScoped<IDeliveryService, DeliveryService>();
            services.AddAutoMapper(typeof(MapperEntities).Assembly);

            return services;
        }
    }
}
