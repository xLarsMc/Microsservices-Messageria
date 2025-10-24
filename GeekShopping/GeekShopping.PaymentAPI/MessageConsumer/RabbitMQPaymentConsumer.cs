using GeekShopping.PaymentAPI.Messages;
using GeekShopping.PaymentAPI.RabbitMQSender;
using GeekShopping.PaymentProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.PaymentAPI.MessageConsumer
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private IConnection _connection;
        private IChannel _channel;
        private IRabbitMQMessageSender _rabbitMQMessageSender;
        private IProcessPayment _processPayment;

        public RabbitMQPaymentConsumer(IProcessPayment processPayment, IRabbitMQMessageSender rabbitMQMessageSender)
        {
            _processPayment = processPayment;
            _rabbitMQMessageSender = rabbitMQMessageSender;
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

            await _channel.QueueDeclareAsync(queue: "orderpaymentprocessqueue", false, false, false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (chanel, evt) =>
            {
                var content = Encoding.UTF8.GetString(evt.Body.ToArray());
                PaymentMessageDTO dto = JsonSerializer.Deserialize<PaymentMessageDTO>(content);
                await ProcessPayment(dto);
                await _channel.BasicAckAsync(evt.DeliveryTag, false);
            };

            await _channel.BasicConsumeAsync("orderpaymentprocessqueue", false, consumer);

        }

        private async Task ProcessPayment(PaymentMessageDTO dto)
        {
            var result = _processPayment.PaymentProcessor();

            UpdatePaymentResultMessage paymentResult = new()
            {
                Status = result,
                OrderId = dto.OrderId,
                Email = dto.Email
            };

            try
            {
                _rabbitMQMessageSender.SendMessage(paymentResult);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
