using AutoMapper;
using Edrak.Order.Data.Entities;
using Edrak.Order.Models.Contracts;
using Edrak.Order.Models.DTOs;
using Edrak.Order.Models.EntityDTOs;
using Newtonsoft.Json;
using OrderStatusEnum = Edrak.Order.Models.Enums.OrderStatus;

namespace Edrak.Order.API.MappingProfile
{
    public class OrdersMappingProfile : Profile
    {
        public OrdersMappingProfile()
        {

            #region Map Dto to Entity
            MapCreateOrderDTOToOrderEntity();
            MapOrderProductDTOToOrderLineItem();
            #endregion

            #region Map Entity to Dto
            MapOrderEntityToOrderDTO();
            MapOrderLineItemEntityToOrderLineItemDTO();
            MapProductEntityTProductDTO();
            #endregion

            #region Map Dto to Response
            MapCustomerDTOToCustomerResponse();
            MapOrderLineItemDTOToOrderItemResponse();
            MapOrderDTOToOrderDetailsResponse();
            MapOrderDTOToOrderResponse();
            #endregion

            #region Map Enum to Dto

            #endregion
            MapProductDTOToOrderProductMetaDataDTO();
            MapOrderLineItemToOrderProductDTO();
        }

        #region Map Dto To Entity
        void MapCreateOrderDTOToOrderEntity()
        {
            CreateMap<CreateOrderDTO, Data.Entities.Order>()
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => OrderStatusEnum.Pending))
                .ForMember(dest => dest.OrderLineItems, opt => opt.MapFrom(src => src.Products))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                ;
        }
        void MapOrderProductDTOToOrderLineItem()
        {
            CreateMap<OrderProductDTO, OrderLineItem>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                ;
        }
        #endregion

        #region Map Entity To Dto
        void MapOrderEntityToOrderDTO()
        {
            CreateMap<Data.Entities.Order, OrderDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.StatusId))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
                ;
        }
        void MapOrderLineItemEntityToOrderLineItemDTO()
        {
            CreateMap<OrderLineItem, OrderLineItemDTO>()
                .ForMember(dest => dest.ProductMetaData, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<OrderProductMetaDataDTO>(src.ProductMetaData)))
                ;
        }
        void MapProductEntityTProductDTO()
        {
            CreateMap<Product, ProductDTO>()
                ;
        }
        #endregion

        #region Map Dto to Response
        void MapOrderDTOToOrderResponse()
        {
            CreateMap<OrderDTO, OrderResponse>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderLineItems))
                ;
        }
        void MapOrderDTOToOrderDetailsResponse()
        {
            CreateMap<OrderDTO, OrderDetailsResponse>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderLineItems))
                ;
        }
        void MapOrderLineItemDTOToOrderItemResponse()
        {
            CreateMap<OrderLineItemDTO, OrderItemResponse>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.ProductMetaData.Price))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductMetaData.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ProductMetaData.Description))
                ;
        }
        void MapCustomerDTOToCustomerResponse()
        {
            CreateMap<CustomerDTO, CustomerResponse>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Id))
                ;
        }
        #endregion

        #region Map Enum To Dto
        #endregion

        void MapProductDTOToOrderProductMetaDataDTO()
        {
            CreateMap<ProductDTO, OrderProductMetaDataDTO>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id));
        }
        void MapOrderLineItemToOrderProductDTO()
        {
            CreateMap<OrderLineItem, OrderProductDTO>();
        }
    }
}
