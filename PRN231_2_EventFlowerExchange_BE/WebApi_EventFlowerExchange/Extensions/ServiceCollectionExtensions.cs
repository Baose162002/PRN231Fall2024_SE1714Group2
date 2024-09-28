﻿
using BusinessObject.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Repository.IRepository;
using Repository.Repository;
using Service.IService;
using Service.Service;
using System.Text.Json.Serialization;

namespace RBN_Api.Extensions
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


            // Register services here
            services.AddScoped<IBatchService, BatchService>();

            return services;
        }
    }
}
