using eCommerce.BusinessLogicLayer.DTO;
using FluentValidation;

namespace eCommerce.ProductsMicroService.API.APIEndpoints;
using eCommerce.BusinessLogicLayer.ServiceContracts;

public static class ProductAPIEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder builder)
    {
        //GET /api/products
        builder.MapGet("api/products", async (IProductsService productService) =>
        {
            var products = await productService.GetProducts();
            return Results.Ok(products);
        });
        
        //GET /api/products/search/product-id/{ProductID:guid}
        builder.MapGet("api/products/search/product-id/{ProductID:guid}",
            async (IProductsService productService, Guid ProductID) =>
            {
                var product = await productService.GetProductByCondition(p => p.ProductID == ProductID);
                return Results.Ok(product);
            });
        
        //GET /api/products/search/{SearchString}
        builder.MapGet("api/products/search/{SearchString}",
            async (IProductsService productService, string SearchString) =>
            {
                var productsByProductName = await productService.GetProductsByCondition(p => p.ProductName != null && p.ProductName.Contains(SearchString, StringComparison.OrdinalIgnoreCase));
                var productsByProductCategory = await productService.GetProductsByCondition(p => p.Category != null && p.Category.Contains(SearchString, StringComparison.OrdinalIgnoreCase));
                var products = productsByProductName.Union(productsByProductCategory).ToList();
                return Results.Ok(products);
            });

        //POST /api/products
        builder.MapPost("api/products",
            async (IProductsService productService, IValidator<ProductAddRequest> productAddRequestValidator,
                ProductAddRequest productAddRequest) =>
            {
                var validationError = await productAddRequestValidator.ValidateAsync(productAddRequest);
                if (!validationError.IsValid)
                {
                    Dictionary<string, string[]> errorDict = validationError.Errors.GroupBy(x => x.PropertyName)
                        .ToDictionary(x => x.Key, x => x.Select(y => y.ErrorMessage).ToArray());
                    return Results.ValidationProblem(errorDict);
                }
                
                var createdProductResponse = await productService.AddProduct(productAddRequest);
                
                if (createdProductResponse != null)
                    return Results.Created($"/api/products/search/product-id/{createdProductResponse.ProductID}", createdProductResponse);
                else
                    return Results.Problem("Error in adding product");
            });
        

        //PUT /api/products
        builder.MapPut("/api/products", async (IProductsService productsService, IValidator<ProductUpdateRequest> productUpdateRequestValidator, ProductUpdateRequest productUpdateRequest) =>
        {
            //Validate the ProductUpdateRequest object using Fluent Validation
            var validationResult = await productUpdateRequestValidator.ValidateAsync(productUpdateRequest);

            //Check the validation result
            if (!validationResult.IsValid)
            {
                Dictionary<string, string[]> errors = validationResult.Errors
                    .GroupBy(temp => temp.PropertyName)
                    .ToDictionary(grp => grp.Key,
                        grp => grp.Select(err => err.ErrorMessage).ToArray());
                return Results.ValidationProblem(errors);
            }


            var updatedProductResponse = await productsService.UpdateProduct(productUpdateRequest);
            if (updatedProductResponse != null)
                return Results.Ok(updatedProductResponse);
            else
                return Results.Problem("Error in updating product");
        });


        //DELETE /api/products/xxxxxxxxxxxxxxxxxxx
        builder.MapDelete("/api/products/{ProductID:guid}", async (IProductsService productsService, Guid ProductID) =>
        {
            bool isDeleted = await productsService.DeleteProduct(ProductID);
            if (isDeleted)
                return Results.Ok(true);
            else
                return Results.Problem("Error in deleting product");
        });

        return builder;
    }
}