using AutoMapper;
using WiseWMS.Application.DTOs;
using WiseWMS.Infrastructure.Entities;

namespace WiseWMS.Application.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty));

        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();

        CreateMap<Customer, CustomerDto>();
        CreateMap<CreateCustomerDto, Customer>();
        CreateMap<UpdateCustomerDto, Customer>();

        CreateMap<Supplier, SupplierDto>();
        CreateMap<CreateSupplierDto, Supplier>();
        CreateMap<UpdateSupplierDto, Supplier>();

        CreateMap<InboundOrder, InboundOrderDto>()
            .ForMember(d => d.SupplierName, o => o.MapFrom(s => s.Supplier != null ? s.Supplier.Name : string.Empty))
            .ForMember(d => d.OperatorName, o => o.MapFrom(s => s.Operator != null ? s.Operator.DisplayName : string.Empty));

        CreateMap<InboundItem, InboundItemDto>()
            .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product != null ? s.Product.Name : string.Empty))
            .ForMember(d => d.ProductSpec, o => o.MapFrom(s => s.Product != null ? s.Product.Spec : string.Empty));

        CreateMap<CreateInboundDto, InboundOrder>();
        CreateMap<CreateInboundItemDto, InboundItem>();

        CreateMap<OutboundOrder, OutboundOrderDto>()
            .ForMember(d => d.CustomerName, o => o.MapFrom(s => s.Customer != null ? s.Customer.Name : string.Empty))
            .ForMember(d => d.OperatorName, o => o.MapFrom(s => s.Operator != null ? s.Operator.DisplayName : string.Empty));

        CreateMap<OutboundItem, OutboundItemDto>()
            .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product != null ? s.Product.Name : string.Empty))
            .ForMember(d => d.ProductSpec, o => o.MapFrom(s => s.Product != null ? s.Product.Spec : string.Empty));

        CreateMap<CreateOutboundDto, OutboundOrder>();
        CreateMap<CreateOutboundItemDto, OutboundItem>();

        CreateMap<InventoryTransaction, InventoryTransactionDto>()
            .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product != null ? s.Product.Name : string.Empty))
            .ForMember(d => d.ProductSpec, o => o.MapFrom(s => s.Product != null ? s.Product.Spec : string.Empty))
            .ForMember(d => d.OperatorName, o => o.MapFrom(s => s.Operator != null ? s.Operator.DisplayName : string.Empty));
    }
}
