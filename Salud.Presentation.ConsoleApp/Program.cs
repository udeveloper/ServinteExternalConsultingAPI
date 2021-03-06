﻿using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Salud.Framework.Broker.Core;
using Salud.Framework.Broker.Core.ConfigurationModel;
using Salud.Framework.CosmosDB.Core;
using Servinte.Framework.NotificationEvents.Core;

namespace Salud.Presentation.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // ADD THIS PART TO YOUR CODE
            try
            {
                
                Program p = new Program();
                //p.GetStartedSignalR();
               //p.GetStartedBroker().Wait();
                p.GetStartedDemo().Wait();
            }           
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }

        // ADD THIS PART TO YOUR CODE
        private async Task GetStartedDemo()
        {
            IStoragePersistent storagePersistentCosmosDB = new StoragePersistentCosmosDB();
            await  storagePersistentCosmosDB.Connect();
            //await storagePersistentCosmosDB.CreateDatabase("ExternalConsulting");
            //await storagePersistentCosmosDB.CreateEntity("ExternalConsulting", "RecordPatientCollection");

            //string document = "{'id':'1','nombre':'ABRAHAN URIEL OLAUA','tipoIdentificacion':'CC','numeroIdentificacion':91533079,'edad':34,'peso':74.0,'masaCorporal':24.8,'superficieCorporal':1.9,'genero':'M','identificador':57977,'talla':174.0,'grupoSanguineo':'O + '}";
            //await storagePersistentCosmosDB.CreateDocumentEntity("ExternalConsulting", "RecordPatientCollection",document);

            dynamic documento=  storagePersistentCosmosDB.QueryDocumentEntity("ExternalConsulting", "IndexationCollection", "IndexationCollection.idEpisodio", "397497898");

            string doc = JsonConvert.SerializeObject(documento);

            Console.WriteLine(doc);
        }

        private void GetStartedSignalR()
        {
            RabbitMQConsumerResponses client = new RabbitMQConsumerResponses();
            client.CreateConnection();
            client.ProcessMessages();
        }

        private async Task GetStartedBroker()
        {
            IBrokerClient rabbitMQBrokerClient = new RabbitMQBrokerClient("40.121.32.117", "developerAdmin","developerAdmin");

            ConfigurationPublisherClient publisher = new ConfigurationPublisherClient
            {
                Applicacion = "ExternalConsulting",
                Module = "Diagnostic",
                Action = "Update",
                DocumentName = "indexation"

            };

           // string json =await System.IO.File.ReadAllTextAsync(@"C:\Users\developerAdmin\Downloads\sincronizacion.txt");
            string json = await System.IO.File.ReadAllTextAsync(@"C:\Users\developerAdmin\Downloads\899999017-5415848-0004-005_data.json");


            for (int i=0; i<1000;i++)
            {
                try
                {
                    bool send = rabbitMQBrokerClient.SendMessage<object,object>(JsonConvert.DeserializeObject(json),null, publisher);
                    Console.WriteLine(i.ToString() );
                }
                catch { }
                              
            }

            Console.WriteLine(DateTime.Now);
            Console.Read();
           
        }
    }
}
