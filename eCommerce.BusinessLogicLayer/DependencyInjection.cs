using eCommerce.BusinessLogicLayer.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.BusinessLogicLayer;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
    {
        // Register your business logic layer services here
        services.AddAutoMapper(typeof(ProductUpdateRequestToProductMappingProfile).Assembly);
        return services;
    }
}