using GeekShopping.MessageBus;
using GeekShopping.OrderAPI.Messages;
using GeekShopping.OrderAPI.Model;
using GeekShopping.OrderAPI.Repository;
using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;

namespace GeekShopping.OrderAPI.MessageConsumer
{
    public class RabbitMQCheckoutConsumer : BackgroundService
    {
        private readonly OrderRepository _repository;
        private IConnection _connection;
        private IChannel _channel;

        public RabbitMQCheckoutConsumer(OrderRepository repository)
        {
            _repository = repository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(queue: "checkoutqueue", false, false, false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (chanel, evt) =>
            {
                var content = Encoding.UTF8.GetString(evt.Body.ToArray());
                CheckoutHeaderDTO dto = JsonSerializer.Deserialize<CheckoutHeaderDTO>(content);
                await ProcessOrder(dto);
                await _channel.BasicAckAsync(evt.DeliveryTag, false);
            };

            await _channel.BasicConsumeAsync("checkoutqueue", false, consumer);

        }

        private async Task ProcessOrder(CheckoutHeaderDTO dto)
        {
            OrderHeader order = new()
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                OrderDetails = new List<OrderDetail>(),
                CardNumber = dto.CardNumber,
                CouponCode = dto.CouponCode,
                CVV = dto.CVV,
                DiscountAmount = dto.DiscountAmount,
                Email = dto.Email,
                ExpiryMonthYear = dto.ExpiryMonthYear,
                OrderTime = DateTime.Now,
                PurchaseAmount = dto.PurchaseAmount,
                PaymentStatus = false,
                PhoneNumber = dto.PhoneNumber,
                DateTime = dto.DateTime
            };

            foreach(var details in dto.CartDetails)
            {
                OrderDetail detail = new()
                {
                    ProductId = details.ProductId,
                    ProductName = details.product.Name,
                    Price = details.product.Price,
                    Count = details.Count
                };
                order.CartTotalItens += details.Count;
                order.OrderDetails.Add(detail);
            }

            await _repository.AddOrder(order);
        }
    }
}
