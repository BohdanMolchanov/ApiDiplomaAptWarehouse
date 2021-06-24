using System;
using AutoMapper;
using Diploma.Apt.Warehouse.Core.Data.Entities.MongoDB;
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

            CreateMap<UserEntity, AuthResponseModel>()
                .ForMember(x => x.Email, o
                    => o.MapFrom(s => s.Data.Email))
                .ForMember(x => x.Phone, o
                    => o.MapFrom(s => s.Data.Phone))
                .ForMember(x => x.FirstName, o
                    => o.MapFrom(s => s.Data.FirstName))
                .ForMember(x => x.LastName, o
                    => o.MapFrom(s => s.Data.LastName))
                .ForMember(x => x.Id, o
                    => o.MapFrom(s => s.Id))
                .ForMember(x => x.RoleType, o
                    => o.MapFrom(s => s.Data.RoleType));

            CreateMap<CreateOrganizationRequestModel, OrganizationEntity>();
            CreateMap<CreateDepartmentRequestModel, DepartmentEntity>();
            CreateMap<CreateUserRequestModel, UserEntity>();

            CreateMap<RegisterOrganizationRequest, CreateUserRequestModel>();
            CreateMap<RegisterOrganizationRequest, CreateOrganizationRequestModel>();

            CreateMap<OrganizationEntity, OrganizationResponseModel>()
                .ForMember(x => x.CreatedAt, o =>
                {
                    o.PreCondition(s => s.CreatedAt.HasValue);
                    o.MapFrom(s => s.CreatedAt.Value.ToString("dd.MM.yyyy"));
                });
            CreateMap<DepartmentEntity, DepartmentResponseModel>()
                .ForMember(x => x.CreatedAt, o =>
                {
                    o.PreCondition(s => s.CreatedAt.HasValue);
                    o.MapFrom(s => s.CreatedAt.Value.ToString("dd.MM.yyyy"));
                });
            CreateMap<UserEntity, UserResponseModel>()
                .ForMember(x => x.CreatedAt, o =>
                {
                    o.PreCondition(s => s.CreatedAt.HasValue);
                    o.MapFrom(s => s.CreatedAt.Value.ToString("dd.MM.yyyy"));
                });

            /*CreateMap<ProductEntity, ProductResponseModel>()
                .ForMember(x => x.CreatedAt, o =>
                {
                    o.MapFrom(s => s.CreatedAt.ToString("dd.MM.yyyy"));
                });*/
        }
    }
}