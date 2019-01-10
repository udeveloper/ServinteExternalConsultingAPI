using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Salud.Framework.CosmosDB.Core;

namespace Servinte.Framework.Storage.API.Controllers
{
    [Route("api/[controller]")]
    public class StoragePersistentController : Controller
    {
        private readonly IStoragePersistent storagePersistent;

        public StoragePersistentController(IStoragePersistent storagePersistent)
        {
            this.storagePersistent = storagePersistent;
        }

        [HttpGet("{entityName}/{idKey}/{idDocument}")]
        public async Task<dynamic> Get(string entityName, string idKey, string idDocument)
        {
            IStoragePersistent storagePersistentCosmosDB = new StoragePersistentCosmosDB();
            await storagePersistentCosmosDB.Connect();

            var documentStorage = storagePersistentCosmosDB.QueryDocumentEntity("ExternalConsulting", entityName, entityName + "." + idKey, idDocument);

            var document = JsonConvert.SerializeObject(documentStorage);

            return Ok(document);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] dynamic dynamic)
        {

            //if(dynamic is String )
            //     dynamic = JsonConvert.DeserializeObject(dynamic);        

            var application = dynamic.configuration.application.Value.ToString().ToUpper();

            await storagePersistent.Connect();
            await storagePersistent.CreateDatabase(application);
            await storagePersistent.CreateEntity(application, dynamic.configuration.entityName.Value);

            //var document = JsonConvert.SerializeObject(dynamic[dynamic.configuration.documentName.Value]);
            var document = JsonConvert.SerializeObject(dynamic);
            await storagePersistent.CreateDocumentEntity(application, dynamic.configuration.entityName.Value, document);

            return Ok(dynamic);
        }

        [HttpPut("{idDocument}")]
        public async Task<IActionResult> Put(string idDocument,[FromBody] dynamic dynamic)
        {

            var application = dynamic.configuration.application.Value.ToString().ToUpper();

            await storagePersistent.Connect();

            //var document = JsonConvert.SerializeObject(dynamic[dynamic.configuration.documentName.Value]);
            var document = JsonConvert.SerializeObject(dynamic);
            var response = await storagePersistent.ReplaceDocumentEntity(application,dynamic.configuration.entityName.Value, document,idDocument);

            return Ok();
        }

        public static bool PropertyExists(dynamic obj, string name)
        {
            if (obj == null) return false;
            if (obj is IDictionary<string, object> dict)
            {
                return dict.ContainsKey(name);
            }
            return obj.GetType().GetProperty(name) != null;
        }
    }
}