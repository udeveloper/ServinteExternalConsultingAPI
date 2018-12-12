using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Salud.Framework.Broker.Core.ConfigurationModel;
using Servinte.Framework.Clinic.BasicInformation.Infraestructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Salud.Framework.Broker.Core
{
    public class RabbitMQBrokerClient : IBrokerClient, IDisposable

    {
        private static ConnectionFactory _factory;
        private static IConnection _connection;
        private static IModel _channel;        
        private static List<ConfigurationPublisher> _configurationPublishers;
        private readonly string password;
        private readonly string hostName;
        private readonly string userName;

        public RabbitMQBrokerClient() { }
        public RabbitMQBrokerClient(string hostName,string userName,string password)
        {                             
            this.password = password;
            this.userName = userName;
            this.hostName = hostName;

            CreateConfiguration();
        }

        public void CreateConfiguration()
        {
            _factory = new ConnectionFactory()
            {
                HostName =this.hostName,
                UserName = this.userName,
                Password = this.password
            };

            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            var optionsBuilder = new DbContextOptionsBuilder<ExternalConsultingContext>();
            optionsBuilder.UseSqlite("data source=ExternalConsulting.db");


            using (var context = new ExternalConsultingContext(optionsBuilder.Options))
            {
                _configurationPublishers = context.ConfigurationPublishers.ToList();

                foreach (var configurationPublisher in _configurationPublishers)
                {
                    ConfigurationPublisherServer publisher = new ConfigurationPublisherServer
                    {
                        ExchageName = configurationPublisher.ExchageName,
                        ExchangeType = configurationPublisher.ExchangeType,
                        QueueConfiguration = new Queue
                        {
                            QueueName = configurationPublisher.QueueName,
                            Durable = true,
                            Exclusive = false,
                            AutoDelete = false,
                            Arguments = null
                        },
                        KeyBinding = configurationPublisher.KeyBinding
                    };

                    ConfigureBindingBroker(publisher);
                }
            }

        }

        
        private static void ConfigureBindingBroker(ConfigurationPublisherServer configurationPublisher)
        {
            _channel.ExchangeDeclare(configurationPublisher.ExchageName, configurationPublisher.ExchangeType);

            _channel.QueueDeclare(queue: configurationPublisher.QueueConfiguration.QueueName,
                          durable: configurationPublisher.QueueConfiguration.Durable,
                          exclusive: configurationPublisher.QueueConfiguration.Exclusive,
                          autoDelete: configurationPublisher.QueueConfiguration.AutoDelete,
                          arguments: configurationPublisher.QueueConfiguration.Arguments);

            _channel.QueueBind(queue: configurationPublisher.QueueConfiguration.QueueName,
                               exchange: configurationPublisher.ExchageName,
                               routingKey: configurationPublisher.KeyBinding);
        }

        public bool SendMessage<T,V>(T message, V configurationMessage, ConfigurationPublisherClient configurationPublisher)
        {

            var publisher = _configurationPublishers.SingleOrDefault(c => c.Applicacion == configurationPublisher.Applicacion
                                  && c.Module == configurationPublisher.Module && c.Action == configurationPublisher.Action);

            var properties = _channel.CreateBasicProperties();
            properties.Headers = (configurationPublisher.PropertiesCustom ?? new Dictionary<string, object>());            
            properties.Headers.Add("application", configurationPublisher.Applicacion);
            properties.Headers.Add("module", configurationPublisher.Module);
            properties.Headers.Add("documentName", configurationPublisher.DocumentName);
            properties.Headers.Add("callbackResponse", true);

            if (configurationMessage != null)
                properties.Headers.Add("configuration", configurationMessage);


            _channel.BasicPublish(exchange: publisher.ExchageName,
                                  routingKey: publisher.KeyRouting,
                                  basicProperties: properties,
                                  body: message.Serialize());

            return true;
        }

        public void Dispose()
        {
            if (_connection.IsOpen)
                _connection.Close();
        }
    }
}
