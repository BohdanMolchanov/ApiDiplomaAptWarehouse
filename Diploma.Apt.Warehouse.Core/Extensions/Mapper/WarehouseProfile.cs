using System;
using AutoMapper;
using Diploma.Apt.Warehouse.Core.Data.Entities.PostgreSQL;
using Diploma.Apt.Warehouse.Core.Models.RequestModels;
using Diploma.Apt.Warehouse.Core.Models.ResponseModels;

namespace Diploma.Apt.Warehouse.Core.Extensions.Mapper
{
    public class WarehouseProfile : Profile
    {
        public WarehouseProfile()
        {
            CreateMap<StockEntity, StockResponseModel>()
                .ForMember(x => x.Name, o
                    => o.MapFrom(x => x.Product.NameUkr))
                .ForMember(x => x.Details, o
                    => o.MapFrom(x => x.Product.Description))
                .ReverseMap()
                ;

            CreateMap<ProductEntity, StockResponseModel>()
                .ForMember(x => x.Name, o
                    => o.MapFrom(x => x.NameUkr))
                .ForMember(x => x.Details, o
                    => o.MapFrom(x => x.Description))
                .ReverseMap();
            
            CreateMap<CreateStockRequestModel, StockEntity>();
            CreateMap<CreateProductRequestModel, ProductEntity>();
            CreateMap<ProductEntity, ProductSearchResponseModel>();
            CreateMap<CreateBatchRequestModel, BatchEntity>();
        }
    }
}