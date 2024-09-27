using AutoMapper;
using OnlineShop.BLL.Dtos.Create;
using OnlineShop.BLL.Dtos.Read;
using OnlineShop.BLL.Dtos.Update;
using OnlineShop.DataLayer.Entities;

namespace BLL.Mapping;
public class AutomapperBLLProfile : Profile
{
    public AutomapperBLLProfile()
    {
        #region ItemMapping
        CreateMap<Item, ItemDto>()
            .ForMember(
                dest => dest.ImagesUrls,
                opt => opt.Ignore())
            .ReverseMap();

        CreateMap<Item, CreateItemDto>()
                .ForMember(
                    dest => dest.CategoryIds,
                    opt => opt.MapFrom(src => src.Categories.Select(c => c.Id).ToList()))
                .ForMember(
                    dest => dest.ImageFiles,
                    opt => opt.Ignore())
                .ReverseMap();

        CreateMap<Item, UpdateItemDto>()
            .ForMember(
                dest => dest.CategoryIds,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.ImageFiles, 
                opt => opt.Ignore())
            .ReverseMap();
        #endregion

        #region CategoryMapping
        CreateMap<Category, CategoryDto>()
            .ReverseMap();

        CreateMap<Category, CreateCategoryDto>()
            .ReverseMap();

        CreateMap<Category, UpdateCategoryDto>()
            .ReverseMap();
        #endregion

        #region ItemImageMapping

        #endregion
    }
}
