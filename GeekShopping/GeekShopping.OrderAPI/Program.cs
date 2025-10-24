using GeekShopping.OrderAPI.MessageConsumer;
using GeekShopping.OrderAPI.Model.Context;
using GeekShopping.OrderAPI.RabbitMQSender;
using GeekShopping.OrderAPI.Repository;
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
        Title = "GeekShopping.OrderAPI",
        Version = "v1",
        Description = "API Microsserviço - Order"
    });
});

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = new MySqlServerVersion(new Version(8, 4, 6));

builder.Services.AddDbContext<MySQLContext>(opt =>
                                      opt.UseMySql(connection, serverVersion));

var builderDB = new DbContextOptionsBuilder<MySQLContext>();
builderDB.UseMySql(connection,
    new MySqlServerVersion(new Version(8, 0, 5)));

builder.Services.AddSingleton(new OrderRepository(builderDB.Options));

builder.Services.AddHostedService<RabbitMQCheckoutConsumer>();
builder.Services.AddHostedService<RabbitMQPaymentConsumer>();

builder.Services.AddSingleton<IRabbitMQMessageSender, RabbitMQMessageSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("http://localhost:4451/swagger/v1/swagger.json", "GeekShopping.OrderAPI v1");
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();
