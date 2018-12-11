using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;
using System.Net.Http;
using System.Dynamic;
using System.Collections.Generic;
using IConnection = RabbitMQ.Client.IConnection;
using Salud.Framework.Broker.Core.ConfigurationModel;
using Salud.Framework.Broker.Core;

namespace Servinte.Framework.Broker.Consumer.RabbitMQ
{
    public class RabbitMQConsumer
    {
        private static ConnectionFactory _factory;
        private static IConnection _connection;
        private static HttpClient _httpClient;
        private static IBrokerClient rabbitMQBrokerClient;

        private  string ExchangeName = "exchange_transactions_externalConsulting";
        private  string MonitoringQueueName = "queue_servinte_externalConsulting_transactions_all";        

        public RabbitMQConsumer(string exchangeName,string monitoringQueueName)
        {
            this.ExchangeName = exchangeName;
            this.MonitoringQueueName = monitoringQueueName;
        }

        public void CreateConnection()
        {
            _factory = new ConnectionFactory
            {
                HostName = "137.135.105.219",
                UserName = "developerAdmin",
                Password = "developerAdmin"
            };

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://servinteframeworkstorageapi.azurewebsites.net")
            };

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
                    Console.WriteLine("Listening for Topic <{0}>",this.MonitoringQueueName);
                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine();

                    channel.ExchangeDeclare(ExchangeName, "topic");
                    channel.QueueDeclare(MonitoringQueueName, 
                        true, false, false, null);

                    channel.QueueBind(MonitoringQueueName, ExchangeName, "");                    
                    

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
                        //var message = (Patient)deliveryArguments.Body.DeSerialize(typeof(Patient));                       

                        var routingKey = deliveryArguments.RoutingKey;


                        //Adicionar Logica Personalidada por cada Clave de Routing y entidad.

                        //var response = await _httpClient.PostAsJsonAsync("/api/storagepersistent",CustomLogic(message));  

                        bool responseSuccess=false;

                        try
                        {


                            var response = await _httpClient.PostAsJsonAsync("api/storagepersistent", CustomLogic(message,deliveryArguments));

                            if (response.IsSuccessStatusCode)
                            {
                                subscription.Ack(deliveryArguments);

                                ConfigurationPublisherClient publisher = new ConfigurationPublisherClient
                                {
                                    Applicacion = "ExternalConsulting",
                                    Module = "Basic_information",
                                    Action = "Add",
                                    DocumentName = "Patient"

                                };
                                rabbitMQBrokerClient.SendMessage<string>("Mensaje PUblicado " + deliveryArguments.DeliveryTag, publisher);

                            }

                            responseSuccess = response.IsSuccessStatusCode;
                          
                        }
                        catch(Exception ex)  {
                             
                        }
                        finally
                        {
                            Console.WriteLine("--- Payment - Routing Key <{0}> : {1} - Estado : {2} ", routingKey, deliveryArguments.DeliveryTag, responseSuccess);
                        }

                                                                  

                    }
                }
            }
        }

        private object CustomLogic(object message, BasicDeliverEventArgs dataEvent)
        {

            IBasicProperties properties = dataEvent.BasicProperties;

            // Create a dynamic output object
            dynamic output = new ExpandoObject();
            output.configuration = new ExpandoObject();
            output.configuration.application = Encoding.Default.GetString(((Byte[])properties.Headers["application"]));
            output.configuration.entityName = Encoding.Default.GetString(((Byte[])properties.Headers["module"]));
            output.configuration.documentName = Encoding.Default.GetString(((Byte[])properties.Headers["documentName"]));
            //output.entity = new ExpandoObject();
            var x = output as IDictionary<string, Object>;
            x.Add(output.configuration.documentName, new ExpandoObject());
            x[output.configuration.documentName] = message;
            //output.entity = message;

           // string outputJson = JsonConvert.SerializeObject(output, Formatting.Indented);
            
            return output;
        }
    }
}
