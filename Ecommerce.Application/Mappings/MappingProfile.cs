using AutoMapper;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // এখানে পরে DTO তৈরি হলে CreateMap() যোগ করবো
            // যেমন: CreateMap<Product, ProductDto>();
            //       CreateMap<CreateProductDto, Product>();
        }
    }
}