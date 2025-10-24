using GeekShopping.OrderAPI.Messages;
using GeekShopping.OrderAPI.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.OrderAPI.MessageConsumer
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private readonly OrderRepository _repository;
        private IConnection _connection;
        private IChannel _channel;
        private const string _exchangeName = "FanoutPaymentExchangeUpdate";
        private string _queueName = "";

        public RabbitMQPaymentConsumer(OrderRepository repository)
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

            await _channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Fanout);
            _queueName = _channel.QueueDeclareAsync().Result.QueueName;
            await _channel.QueueBindAsync(_queueName, _exchangeName, "");

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (chanel, evt) =>
            {
                var content = Encoding.UTF8.GetString(evt.Body.ToArray());
                UpdatePaymentResultDTO dto = JsonSerializer.Deserialize<UpdatePaymentResultDTO>(content);
                await UpdatePaymentStatus(dto);
                await _channel.BasicAckAsync(evt.DeliveryTag, false);
            };

            await _channel.BasicConsumeAsync(_queueName, false, consumer);

        }

        private async Task UpdatePaymentStatus(UpdatePaymentResultDTO dto)
        {
            try
            {
                await _repository.UpdateOrderPaymentStatus(dto.OrderId, dto.Status);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
