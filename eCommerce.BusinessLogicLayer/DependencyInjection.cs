using eCommerce.BusinessLogicLayer.Mappers;
using eCommerce.BusinessLogicLayer.ServiceContracts;
using eCommerce.BusinessLogicLayer.Services;
using eCommerce.BusinessLogicLayer.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.BusinessLogicLayer;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
    {
        // Register your business logic layer services here
        services.AddAutoMapper(typeof(ProductUpdateRequestToProductMappingProfile).Assembly);

        services.AddValidatorsFromAssemblyContaining <ProductAddRequestValidator>();
        services.AddScoped<IProductsService, ProductService>();
        return services;
    }
}