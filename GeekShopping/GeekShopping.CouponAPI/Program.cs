using GeekShopping.CouponAPI.Config;
using GeekShopping.CouponAPI.Model.Context;
using GeekShopping.CouponAPI.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "GeekShopping.CouponAPI",
        Version = "v1",
        Description = "API Microsserviço - Coupon de desconto"
    });
});

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = new MySqlServerVersion(new Version(8, 4, 6));

builder.Services.AddAutoMapper(typeof(MapperConfig).Assembly);

builder.Services.AddDbContext<MySQLContext>(opt =>
                                      opt.UseMySql(connection, serverVersion));

builder.Services.AddScoped<ICouponRepository, CouponRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("http://localhost:4450/swagger/v1/swagger.json", "GeekShopping.CartAPI v1");
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();
