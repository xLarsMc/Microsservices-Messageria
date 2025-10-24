using GeekShopping.MessageBus;
using GeekShopping.PaymentAPI.Messages;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace GeekShopping.PaymentAPI.RabbitMQSender
{
    public class RabbitMQMessageSender : IRabbitMQMessageSender
    {
        private readonly string _hostName;
        private readonly string _password;
        private readonly string _username;
        private IConnection _connection;
        private const string _exchangeName = "FanoutPaymentExchangeUpdate";

        public RabbitMQMessageSender()
        {
            _hostName = "localhost";
            _password = "guest";
            _username = "guest";
        }

        public async void SendMessage(BaseMessage baseMessage)
        {
            if (await ConectionExists())
            {
                using var channel = await _connection.CreateChannelAsync();
                using var model = await _connection.CreateChannelAsync();

                await channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Fanout, durable: false);

                byte[] body = GetMessageAsByteArray(baseMessage);


                await channel.BasicPublishAsync(
                        exchange: _exchangeName,
                        "",
                        mandatory: false,
                        basicProperties: new BasicProperties { Persistent = true },
                        body: body);
            }
        }

        private byte[] GetMessageAsByteArray(BaseMessage baseMessage)
        {
            var opt = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize<UpdatePaymentResultMessage>((UpdatePaymentResultMessage)baseMessage, opt);

            var body = Encoding.UTF8.GetBytes(json);

            return body;
        }

        private async Task CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostName,
                    UserName = _username,
                    Password = _password,
                };

                _connection = await factory.CreateConnectionAsync();
            } catch(Exception ex)
            {
                throw;
            }
        }

        private async Task<bool> ConectionExists()
        {
            if (_connection != null) return true;
            await CreateConnection();
            return _connection != null;
        }
    }
}
