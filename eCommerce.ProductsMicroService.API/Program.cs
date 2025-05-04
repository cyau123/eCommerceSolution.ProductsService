using eCommerce.BusinessLogicLayer;
using eCommerce.DataAccessLayer;
using FluentValidation.AspNetCore;
using eCommerce.ProductsMicroService.API.Middleware;
using eCommerce.ProductsMicroService.API.APIEndpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddBusinessLogicLayer();

builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();

app.UseExceptionHandlingMiddleware();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapProductEndpoints();
app.Run();