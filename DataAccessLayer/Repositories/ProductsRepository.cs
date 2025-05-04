using System.Linq.Expressions;
using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories;

public class ProductsRepository : IProductsRepository
{
    private readonly ApplicationDbContext _context;
    
    public ProductsRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression)
    {   
        return await _context.Products
            .Where(conditionExpression)
            .ToListAsync();
    }

    public async Task<Product?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression)
    {
        return await _context.Products
            .FirstOrDefaultAsync(conditionExpression);
    }

    public async Task<Product?> AddProduct(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product;
    }

    public async Task<Product?> UpdateProduct(Product product)
    {
        Product? existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == product.ProductID);

        if (existingProduct == null)
        {
            return null;
        }
        
        existingProduct.ProductName = product.ProductName;
        existingProduct.Category = product.Category;
        existingProduct.UnitPrice = product.UnitPrice;
        existingProduct.QuantityInStock = product.QuantityInStock;
        
        await _context.SaveChangesAsync();

        return existingProduct;
    }

    public async Task<bool> DeleteProduct(Guid productId)
    {
        Product? product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == productId);

        if (product == null)
        {
            return false;
        }

        _context.Products.Remove(product);
        var returnedRows = await _context.SaveChangesAsync();

        return returnedRows > 0;    
    }
}