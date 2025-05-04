using DataAccessLayer.Context;
using DataAccessLayer.Repositories;
using DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccessLayer;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        // Register your data access layer services here
        services.AddDbContext<ApplicationDbContext>(option =>
        {
            option.UseMySQL(configuration.GetConnectionString("DefaultConnection")!);
        });
        
        services.AddScoped<IProductsRepository, ProductsRepository>();
        return services;
    }
}