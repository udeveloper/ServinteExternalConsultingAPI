using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Salud.Framework.Broker.Core;
using Salud.Framework.Broker.Core.ConfigurationModel;
using Servinte.Framework.Clinic.BasicInformation.Infraestructure;
using Microsoft.EntityFrameworkCore;

namespace Servinte.Framework.Clinic.BasicInformation.API.Controllers
{
    [Route("api/[controller]")]    
    public class BasicInformationController : Controller
    {
        private readonly ExternalConsultingContext context;
        private readonly IBrokerClient rabbitMQBrokerClient;

        public BasicInformationController(ExternalConsultingContext context,IBrokerClient rabbitMQBrokerClient)
        {
            this.context = context;
            this.rabbitMQBrokerClient = rabbitMQBrokerClient;
            
        }

        // GET: BasicInformation
        [HttpGet]
        public IActionResult Get()
        {
            var patient= this.context.Patients.FirstOrDefault();

            //ConfigurationPublisherClient publisher = new ConfigurationPublisherClient
            //{   
            //    Applicacion= "External Consulting",
            //    Module= "Basic_information",
            //    Action="Add"

                
            //};
            //this.rabbitMQBrokerClient.SendMessage<Patient>(patient, publisher);

            //publisher = new ConfigurationPublisherClient
            //{
            //    Applicacion = "External Consulting",
            //    Module = "Basic_information",
            //    Action = "Update"

            //};
            //this.rabbitMQBrokerClient.SendMessage<Patient>(patient, publisher);

            //publisher = new ConfigurationPublisherClient
            //{
            //    Applicacion = "External Consulting",
            //    Module = "Basic_information",
            //    Action = "Delete"
                
            //};
            //this.rabbitMQBrokerClient.SendMessage<Patient>(patient, publisher);

            //publisher = new ConfigurationPublisherClient
            //{
            //    Applicacion = "External Consulting",
            //    Module = "Diagnostic",
            //    Action = "Add"

            //};
            //this.rabbitMQBrokerClient.SendMessage<Patient>(patient, publisher);

            //publisher = new ConfigurationPublisherClient
            //{
            //    Applicacion = "External Consulting",
            //    Module = "Diagnostic",
            //    Action = "Update"

            //};
            //this.rabbitMQBrokerClient.SendMessage<Patient>(patient, publisher);

            //publisher = new ConfigurationPublisherClient
            //{
            //    Applicacion = "External Consulting",
            //    Module = "Basic_information",
            //    Action = "Delete"

            //};
            //this.rabbitMQBrokerClient.SendMessage<Patient>(patient, publisher);

            return Ok(patient);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var patient = await this.context.Patients.FirstOrDefaultAsync(c=>c.Id==id);

            return Ok(patient);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Patient patient)
        {
            this.context.Patients.Add(patient);
            await this.context.SaveChangesAsync();

            ConfigurationPublisherClient publisher = new ConfigurationPublisherClient
            {
                Applicacion = "ExternalConsulting",
                Module = "Basic_information",
                Action = "Add",
                DocumentName="Patient"

            };
            this.rabbitMQBrokerClient.SendMessage<Patient>(patient, publisher);

            return Ok(patient);
        }

    }
}