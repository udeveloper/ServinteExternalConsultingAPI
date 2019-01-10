using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace Salud.Framework.CosmosDB.Core
{
    public class StoragePersistentCosmosDB : IStoragePersistent
    {

        private const string EndpointUri = "https://servinte.documents.azure.com:443/";
        private const string PrimaryKey = "JIjRbHy6mm354dszd6ELXqU1Agi8zvBz7VpmM7GLJpruqA8b4NmDaCnG7OAnzO7SIZvE9OSOUcyex7r5N9Kczw==";
        private DocumentClient client;
        public Task Connect()
        {
            this.client =  new DocumentClient(new Uri(EndpointUri), PrimaryKey);

            return Task.CompletedTask;
        }

        public async Task CreateDatabase(string databaseName)
        {
             await this.client.CreateDatabaseIfNotExistsAsync(new Database { Id= databaseName });
            
        }

        public async Task CreateDocumentEntity(string databaseName, string entityName, string document)
        {

            await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, entityName), JsonConvert.DeserializeObject(document));
        }

        public async Task CreateEntity(string databaseName, string entityName)
        {
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(databaseName), new DocumentCollection { Id = entityName });
        }

        public async Task<Boolean> ReplaceDocumentEntity(string databaseName, string entityName, string document, string idDocument)
        {
            var response = await this.client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseName, entityName,idDocument), JsonConvert.DeserializeObject(document));

            return (response.StatusCode==HttpStatusCode.OK); 
        }

        public dynamic QueryDocumentEntity(string databaseName, string entityName,string predicate, string idDocument)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = 1 };

            // Now execute the same query via direct SQL
            var document = this.client.CreateDocumentQuery(
                    UriFactory.CreateDocumentCollectionUri(databaseName.ToUpper(), entityName),
                   string.Format("SELECT * FROM {0} where {1} = '{2}'", entityName, predicate, idDocument),
                    queryOptions).AsEnumerable().FirstOrDefault();

            
            return document;
        }

    }
}
