using System.Linq.Expressions;
using AutoMapper;
using eCommerce.BusinessLogicLayer.DTO;
using eCommerce.BusinessLogicLayer.RabbitMQ;
using eCommerce.BusinessLogicLayer.ServiceContracts;
using eCommerce.DataAccessLayer.Entities;
using eCommerce.DataAccessLayer.RepositoryContracts;
using FluentValidation;
using FluentValidation.Results;

namespace eCommerce.BusinessLogicLayer.Services;

public class ProductService : IProductsService
{
    private readonly IMapper _mapper;
    private readonly IProductsRepository _productsRepository;
    private readonly IValidator<ProductAddRequest> _productAddRequestValidator;
    private readonly IValidator<ProductUpdateRequest> _productUpdateRequestValidator;
    private readonly IRabbitMQPublisher _rabbitMQPublisher;
    
    public ProductService(IMapper mapper, IProductsRepository productsRepository, IValidator<ProductAddRequest> productAddRequestValidator, IValidator<ProductUpdateRequest> productUpdateRequestValidator, IRabbitMQPublisher rabbitMqPublisher)
    {
        _mapper = mapper;
        _productsRepository = productsRepository;
        _productAddRequestValidator = productAddRequestValidator;
        _productUpdateRequestValidator = productUpdateRequestValidator;
        _rabbitMQPublisher = rabbitMqPublisher;
    }

    public async Task<List<ProductResponse?>> GetProducts()
    {
        var products = await _productsRepository.GetProducts();
        var productResponses = _mapper.Map<IEnumerable<ProductResponse?>>(products);
        return productResponses.ToList();
    }

    public async Task<List<ProductResponse?>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression)
    {
        var products = await _productsRepository.GetProductsByCondition(conditionExpression);
        var productResponses = _mapper.Map<IEnumerable<ProductResponse?>>(products);
        return productResponses.ToList();
    }

    public async Task<ProductResponse?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression)
    {
        var product = await _productsRepository.GetProductByCondition(conditionExpression);
        
        if (product == null)
        {
            return null;
        }

        ProductResponse productResponse = _mapper.Map<ProductResponse>(product);
        return productResponse;
    }

    public async Task<ProductResponse?> AddProduct(ProductAddRequest productAddRequest)
    {
        if (productAddRequest == null)
        {
            throw new ArgumentNullException(nameof(productAddRequest));
        }
        
        var validationResult = await _productAddRequestValidator.ValidateAsync(productAddRequest);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ArgumentException(errors);
        }
        
        // Map ProductAddRequest to Product entity
        Product product = _mapper.Map<Product>(productAddRequest);
        Product? addedProduct = await _productsRepository.AddProduct(product);

        if (addedProduct == null)
        {
            return null;
        }

        return _mapper.Map<ProductResponse>(addedProduct);
    }

    public async Task<ProductResponse?> UpdateProduct(ProductUpdateRequest productUpdateRequest)
    {
        var existingProduct = await _productsRepository.GetProductByCondition( p => p.ProductID == productUpdateRequest.ProductID);
        if (existingProduct == null)
        {
            throw new ArgumentException("Product not found");
        }
        
        //Validate the product using Fluent Validation
        ValidationResult validationResult = await _productUpdateRequestValidator.ValidateAsync(productUpdateRequest);

        // Check the validation result
        if (!validationResult.IsValid)
        {
            string errors = string.Join(", ", validationResult.Errors.Select(temp => temp.ErrorMessage));
            throw new ArgumentException(errors);
        }
        
        //Map from ProductUpdateRequest to Product type
        Product product = _mapper.Map<Product>(productUpdateRequest);
        
        // Check if product name is changed
        // bool isProductNameChanged = productUpdateRequest.ProductName != existingProduct.ProductName;

        Product? updatedProduct = await _productsRepository.UpdateProduct(product);

        // Publish product.update message to the exchange
        // if (isProductNameChanged)
        // {
            // string routingKey = "product.update.name";
            var headers = new Dictionary<string, object>()
            {
                { "event", "product.update" },
                { "RowCount", 1 }
            };
            // var message = new ProductNameUpdateMessage(product.ProductID, product.ProductName);
            
            // _rabbitMQPublisher.Publish<ProductNameUpdateMessage>(routingKey, message);
            _rabbitMQPublisher.Publish<Product>(headers, product);
        //}
        ProductResponse? updatedProductResponse = _mapper.Map<ProductResponse>(updatedProduct);

        return updatedProductResponse;
    }

    public async Task<bool> DeleteProduct(Guid productID)
    {
        var product = await _productsRepository.GetProductByCondition(p => p.ProductID == productID);
        if (product == null)
        {
            return false;
        }
        var isDeleted =  await _productsRepository.DeleteProduct(productID);
        
        // Publish message of product.delete
        if (isDeleted)
        {
            ProductDeletionMessage message = new ProductDeletionMessage(product.ProductID, product.ProductName);
            // string routingKey = "product.delete";
            var headers = new Dictionary<string, object>() 
            {
                { "event", "product.delete" },
                { "RowCount", 1 }
            };
            // _rabbitMQPublisher.Publish<ProductDeletionMessage>(routingKey, message);
            _rabbitMQPublisher.Publish<ProductDeletionMessage>(headers, message);
        }
        return isDeleted;
    }
}