using BLL.Mapping;
using Microsoft.Extensions.DependencyInjection;
using OnlineShop.BLL.Services.Classes;
using OnlineShop.BLL.Services.Interfaces;

namespace BLL.Extensions;

public static class BLLDI
{
    public static void AddBusinessLogicLayer(
        this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(AutomapperBLLProfile));
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IBlobStorageService, BlobStorageService >();
        services.AddScoped<ITraitService, TraitService>();
    }
}
