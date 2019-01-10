using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Salud.Framework.CosmosDB.Core
{
    public interface IStoragePersistent
    {
        Task Connect();

        Task CreateDatabase(string databaseName);

        Task CreateEntity(string databaseName, string entityName);

        Task CreateDocumentEntity(string databaseName, string entityName, string document);
        Task<Boolean>  ReplaceDocumentEntity(string databaseName, string entityName,string document, string idDocument);

        dynamic QueryDocumentEntity(string databaseName, string entityName, string predicate, string idDocument);
    }
}
