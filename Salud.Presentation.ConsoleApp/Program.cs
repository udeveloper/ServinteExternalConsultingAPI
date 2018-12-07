using System;
using System.Threading.Tasks;
using Salud.Framework.CosmosDB.Core;

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

            await storagePersistentCosmosDB.QueryDocumentEntity("ExternalConsulting", "RecordPatientCollection", "");
        }
    }
}
