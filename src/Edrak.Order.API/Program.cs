using AutoMapper;
using Edrak.Order.API.MappingProfile;
using Edrak.Order.Core.Interfaces;
using Edrak.Order.Core.Services;
using Edrak.Order.Dal.Interfaces;
using Edrak.Order.Dal.Repository;
using Edrak.Order.Dal.Services;
using Edrak.Order.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
services.AddHealthChecks();
string mySQLConnectionString = configuration.GetConnectionString("OrdersDB");
services.AddDbContext<OrdersDBContext>(options => options.UseMySql(mySQLConnectionString,
    ServerVersion.AutoDetect(mySQLConnectionString)));

#region Inject Repository
services.AddScoped(typeof(IOrdersRepository<>), typeof(OrdersRepository<>));
#endregion

#region Inject Core
services.AddScoped<IOrderCore, OrderCore>();
services.AddScoped<IProductCore, ProductCore>();
services.AddScoped<ICustomerCore, CustomerCore>();
#endregion

#region Inject Dal
services.AddScoped<IOrderDal, OrderDal>();
services.AddScoped<IProductDal, ProductDal>();
services.AddScoped<ICustomerDal, CustomerDal>();
#endregion


#region CORS Policy
services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});
#endregion

#region AutoMapper
var mappingConfig = new MapperConfiguration(mc =>
{
    var sp = services.BuildServiceProvider();
    mc.AddProfile(new OrdersMappingProfile());
});
services.AddSingleton(mappingConfig.CreateMapper());
#endregion
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHealthChecks("/hc");
app.UseCors("CorsPolicy");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
