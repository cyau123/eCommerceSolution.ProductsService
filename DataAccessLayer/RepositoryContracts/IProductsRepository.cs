using System.Linq.Expressions;
using DataAccessLayer.Entities;

namespace DataAccessLayer.RepositoryContracts;

/// <summary>
/// Represents a repository for managing the 'products' table.
/// </summary>
public interface IProductsRepository
{
    /// <summary>
    /// Retrieve all products from the database.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<Product>> GetProducts();
    
    /// <summary>
    /// Retrieve a list of products based on the given condition.
    /// </summary>
    /// <param name="conditionExpression"></param>
    /// <returns></returns>
    Task<IEnumerable<Product>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression);
    
    /// <summary>
    /// Retrieve a single product based on the given condition.
    /// </summary>
    /// <param name="conditionExpression"></param>
    /// <returns></returns>
    Task<Product?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression);
    
    /// <summary>
    /// Add a product to the 'products' table
    /// </summary>
    /// <param name="product"></param>
    /// <returns></returns>
    Task<Product?> AddProduct(Product product);
    
    /// <summary>
    /// Update a product in the 'products' table
    /// </summary>
    /// <param name="product"></param>
    /// <returns></returns>
    Task<Product?> UpdateProduct(Product product);
    
    /// <summary>
    ///  Delete a product from the 'products' table
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
    Task<bool> DeleteProduct(Guid productId);
    
}