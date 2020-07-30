using System;
using aspCore.WatchShop.Entities;
using aspCore.WatchShop.Models;
using AutoMapper;

namespace aspCore.WatchShop.Helper
{
    public class ModelMapper : Profile
    {
        public ModelMapper()
        {
            //Promotion
            CreateMap<Promotion, PromotionVM>();
            CreateMap<PromotionVM, Promotion>();
            //Customer
            CreateMap<Customer, CustomerVM>();
            CreateMap<CustomerVM, Customer>();
            //Order
            CreateMap<Order, OrderVM>()
                .ForMember(des => des.Phone, act => act.MapFrom(src => src.Customer.Phone))
                .ForMember(des => des.Address, act => act.MapFrom(src => src.Customer.Address))
                .ForMember(des => des.Customer, act => act.MapFrom(src => src.Customer.Name));
            CreateMap<OrderVM, Order>();
            CreateMap<Order, OrderDetailVM>()
                .ForMember(des => des.Phone, act => act.MapFrom(src => src.Customer.Phone))
                .ForMember(des => des.Address, act => act.MapFrom(src => src.Customer.Address))
                .ForMember(des => des.Customer, act => act.MapFrom(src => src.Customer.Name));
            CreateMap<OrderDetailVM, Order>()
                .ForMember(des => des.Customer, act => act.Ignore());
            CreateMap<OrderDetailVM, Customer>()
                .ForMember(des => des.Name, act => act.MapFrom(src => src.Customer));
            //Order Detail
            CreateMap<OrderDetail, ProductOrder>()
                .ForMember(des => des.ProductName, act => act.MapFrom(src => src.Product.Name))
                .ForMember(des => des.Image, act => act.MapFrom(src => src.Product.Image));
            CreateMap<ProductOrder, OrderDetail>();
            //Wire
            CreateMap<TypeWire, TypeWireVM>();
            CreateMap<TypeWireVM, TypeWire>();

            //Category
            CreateMap<Category, CategoryVM>();
            CreateMap<CategoryVM, Category>();
            //Product
            CreateMap<Product, ProductVM>()
                .ForMember(des => des.Show, act => act.MapFrom(src => src.isShow))
                .ForMember(des => des.Name, act => act.MapFrom(src => src.ProductDetail != null ? src.ProductDetail.Brand + " " + src.Name : src.Name));
            CreateMap<ProductVM, Product>()
                .ForMember(des => des.isShow, act => act.MapFrom(src => src.Show));
            //
            CreateMap<ProductVM, ProductOrder>()
                .ForMember(des => des.ProductID, act => act.MapFrom(src => src.ID))
                .ForMember(des => des.ProductName, act => act.MapFrom(src => src.Name));

            //
            CreateMap<ProductDetailVM, ProductDetail>();
            CreateMap<ProductDetailVM, Product>();
            //
            CreateMap<Product, ProductDetailVM>()
                .ForMember(des => des.Name, act => act.MapFrom(src => src.ProductDetail != null ? src.ProductDetail.Brand + " " + src.Name : src.Name))
                .ForMember(des => des.Images,
                    act => act.MapFrom(src => src.ProductDetail.Images))
                .ForMember(des => des.TypeGlass,
                    act => act.MapFrom(src => src.ProductDetail.TypeGlass))
                .ForMember(des => des.TypeBorder,
                    act => act.MapFrom(src => src.ProductDetail.TypeBorder))
                .ForMember(des => des.TypeMachine,
                    act => act.MapFrom(src => src.ProductDetail.TypeMachine))
                .ForMember(des => des.Parameter,
                    act => act.MapFrom(src => src.ProductDetail.Parameter))
                .ForMember(des => des.ResistWater,
                    act => act.MapFrom(src => src.ProductDetail.ResistWater))
                .ForMember(des => des.Warranty,
                    act => act.MapFrom(src => src.ProductDetail.Warranty))
                .ForMember(des => des.Brand,
                    act => act.MapFrom(src => src.ProductDetail.Brand))
                .ForMember(des => des.Origin,
                    act => act.MapFrom(src => src.ProductDetail.Origin))
                .ForMember(des => des.Color,
                    act => act.MapFrom(src => src.ProductDetail.Color))
                .ForMember(des => des.Func,
                    act => act.MapFrom(src => src.ProductDetail.Func))
                .ForMember(des => des.DescriptionProduct,
                    act => act.MapFrom(src => src.ProductDetail.DescriptionProduct));
        }
    }
}