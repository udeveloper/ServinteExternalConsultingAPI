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
using System.Threading.Tasks;

namespace Servinte.Framework.Broker.Consumer.RabbitMQ
{
    public class RabbitMQConsumer
    {
        //Consumer
        private static ConnectionFactory _factory;
        private static IConnection _connection;
        private static HttpClient _httpClient;

        
        //private static IBrokerClient rabbitMQBrokerClient;

        private  string ExchangeName = "exchange_transactions_externalConsulting";
        private  string MonitoringQueueName = "queue_servinte_externalConsulting_transactions_audit";
        private  string[] keysRouting =new string[] { "servinte.externalConsulting.#" };

        public RabbitMQConsumer(string exchangeName, string monitoringQueueName, string[] keysRouting)
        {
            this.ExchangeName = exchangeName;
            this.MonitoringQueueName = monitoringQueueName;
            this.keysRouting = keysRouting;
        }

        public void CreateConnection()
        {
            _factory = new ConnectionFactory
            {
                HostName = "40.121.32.117",
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
            if(_connection.IsOpen)
                 _connection.Close();
      
        }

        public void ProcessMessages()
        {
            using (_connection = _factory.CreateConnection())
            {
                using (var channel = _connection.CreateModel())
                {
                    Console.WriteLine("Listening for Topic <{0}>",this.MonitoringQueueName);
                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine();

                    channel.ExchangeDeclare(ExchangeName, "topic");
                    channel.QueueDeclare(MonitoringQueueName, true, false, false, null);

                    foreach(var key in this.keysRouting)
                         channel.QueueBind(MonitoringQueueName, ExchangeName, key);                    


                    channel.BasicQos(0, 2, false);
                    
                    Subscription subscription = new Subscription(channel,
                     MonitoringQueueName, false);

                    while (true)
                    {

                        BasicDeliverEventArgs deliveryArguments = subscription.Next();

                        object message = null;

                        if (deliveryArguments != null)
                            message = JsonConvert.DeserializeObject(deliveryArguments.Body.DeSerializeText());

                        var routingKey = deliveryArguments.RoutingKey;


                        //Adicionar Logica Personalidada por cada Clave de Routing y entidad.
                        //var response = await _httpClient.PostAsJsonAsync("/api/storagepersistent",CustomLogic(message));  

                        bool responseSuccess = false;

                        try
                        {


                            if (deliveryArguments.RoutingKey != "servinte.externalConsulting.responses")
                            {
                                _httpClient.PostAsJsonAsync("api/storagepersistent", CustomLogic(message, deliveryArguments)).Wait();
                            }

                            responseSuccess = true;

                        }
                        catch (Exception ex)
                        {

                            Console.WriteLine("--- Payment - Routing Key <{0}> : {1} - Estado : {2} ", routingKey, deliveryArguments.DeliveryTag, ex.Message);
                        }
                        finally
                        {

                            if (responseSuccess)
                            {
                                channel.BasicPublish(exchange: ExchangeName, routingKey: "servinte.responses.externalConsulting",
                                 basicProperties: null, body: message.Serialize());
                                channel.BasicAck(deliveryArguments.DeliveryTag, false);
                            }

                            Console.WriteLine("--- Payment - Routing Key <{0}> : {1} - Estado : {2} ", routingKey, deliveryArguments.DeliveryTag, responseSuccess);
                        }


                    }

                    //consumer.Received += (model, ea) =>
                    //{
                      

                        
                    //};

                    

                    
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
