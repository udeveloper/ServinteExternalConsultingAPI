using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;
using Servinte.Framework.Broker.Consumer.RabbitMQ;
using System;
using System.Threading.Tasks;

namespace Servinte.Framework.NotificationEvents.Core
{
    public class RabbitMQConsumerResponses
    {
        private static ConnectionFactory _factory;
        private static IConnection _connection;        
        private static HubConnection connection;
        //private static IBrokerClient rabbitMQBrokerClient;

        private string ExchangeName = "exchange_transactions_externalConsulting";
        private string MonitoringQueueName = "queue_servinte_externalConsulting_transactions_responses";

     

        public void CreateConnection()
        {
            _factory = new ConnectionFactory
            {
                HostName = "40.121.32.117",
                UserName = "developerAdmin",
                Password = "developerAdmin"
            };

            connection = new HubConnectionBuilder().WithUrl("http://localhost:52038/ChatHub").Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            try
            {
                connection.StartAsync();
            }
            catch (Exception ex)
            {

            }
        }

        public void Close()
        {
            _connection.Close();
        }

        public async void ProcessMessages()
        {
            using (_connection = _factory.CreateConnection())
            {
                using (var channel = _connection.CreateModel())
                {
                    Console.WriteLine("Listening for Topic <{0}>", this.MonitoringQueueName);
                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine();

                    channel.ExchangeDeclare(ExchangeName, "topic");
                    channel.QueueDeclare(MonitoringQueueName,
                        true, false, false, null);
                    
                    channel.BasicQos(0, 2, false);
                    Subscription subscription = new Subscription(channel,
                        MonitoringQueueName, false);

                    while (true)
                    {
                        BasicDeliverEventArgs deliveryArguments = subscription.Next();

                        object message = null;

                        if (deliveryArguments != null)
                            message = JsonConvert.DeserializeObject(deliveryArguments.Body.DeSerializeText());
                        else
                            continue;                                           

                        var routingKey = deliveryArguments.RoutingKey;

                        bool responseSuccess = false;

                        try
                        {


                            await connection.InvokeAsync("SendMessage","Usuario Sistema : Proceso # Finalizo", message.ToString());

                            responseSuccess = true;

                            subscription.Ack(deliveryArguments);
                          

                        }
                        catch (Exception ex)
                        {

                        }
                        finally
                        {
                            Console.WriteLine("--- Payment - Routing Key <{0}> : {1} - Estado : {2} ", routingKey, deliveryArguments.DeliveryTag, responseSuccess);
                        }



                    }
                }
            }
        }

        
    }

}
