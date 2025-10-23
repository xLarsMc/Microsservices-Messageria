using AutoMapper;
using GeekShopping.CartAPI.Config;
using GeekShopping.CartAPI.Model.Context;
using GeekShopping.CartAPI.RabbitMQSender;
using GeekShopping.CartAPI.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // suprimir o filtro automático que retorna 400 antes do action
        options.SuppressModelStateInvalidFilter = true;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "GeekShopping.CartAPI",
        Version = "v1",
        Description = "API Microsserviço - Carrinho de compras"
    });
});

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = new MySqlServerVersion(new Version(8, 4, 6));

builder.Services.AddAutoMapper(typeof(MapperConfig).Assembly);

builder.Services.AddDbContext<MySQLContext>(opt =>
                                        opt.UseMySql(connection, serverVersion));

builder.Services.AddScoped<ICartRepository, CartRepository>();

builder.Services.AddSingleton<IRabbitMQMessageSender, RabbitMQMessageSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("http://localhost:5308/swagger/v1/swagger.json", "GeekShopping.CartAPI v1");
    });

}

app.UseAuthorization();

app.MapControllers();

app.Run();
