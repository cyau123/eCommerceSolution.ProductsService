using eCommerce.DataAccessLayer.Context;
using eCommerce.DataAccessLayer.Repositories;
using eCommerce.DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.DataAccessLayer;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionStringTemplate = configuration.GetConnectionString("DefaultConnection")!;
        string connectionString = connectionStringTemplate
            .Replace("$MYSQL_HOST", Environment.GetEnvironmentVariable("MYSQL_HOST"))
            .Replace("$MYSQL_PASSWORD", Environment.GetEnvironmentVariable("MYSQL_PASSWORD"))
            .Replace("$MYSQL_DATABASE", Environment.GetEnvironmentVariable("MYSQL_DATABASE"))
            .Replace("$MYSQL_PORT", Environment.GetEnvironmentVariable("MYSQL_PORT"))
            .Replace("$MYSQL_USER", Environment.GetEnvironmentVariable("MYSQL_USER"));
            
        // Register your data access layer services here
        services.AddDbContext<ApplicationDbContext>(option =>
        {
            option.UseMySQL(connectionString);
        });
        
        services.AddScoped<IProductsRepository, ProductsRepository>();
        return services;
    }
}