using AutoMapper;
using BusinessObject;
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
            CreateMap<Batch, CreateBatchDTO>();
            CreateMap<Batch, UpdateBatchDTO>()
                .ForMember(dest => dest.BatchStatus, opt => opt.MapFrom(src => src.BatchStatus.ToString()));
            CreateMap<UpdateBatchDTO, Batch>()
               .ForMember(dest => dest.BatchStatus, opt => opt.MapFrom(src => src.BatchStatus.ToString()))
               .ForMember(dest => dest.EntryDate, opt => opt.MapFrom(src => DateTime.ParseExact(src.EntryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)))
               .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => DateTime.ParseExact(src.ExpirationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture))); ;
            CreateMap<ListBatchDTO, Batch>()
                .ForMember(dest => dest.BatchStatus, opt => opt.MapFrom(src => src.BatchStatus.ToString()));
            CreateMap<Batch, ListBatchDTO>()
                .ForMember(dest => dest.BatchStatus, opt => opt.MapFrom(src => src.BatchStatus.ToString()));
            CreateMap<Company, ListCompanyDTO>();
            CreateMap<ListCompanyDTO, Company>();
            CreateMap<Flower, ListFlowerDTO>();
            CreateMap<ListFlowerDTO, Flower>();
        }
    }
}
