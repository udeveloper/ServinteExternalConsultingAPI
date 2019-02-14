using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Salud.Framework.Broker.Core;
using Salud.Framework.Broker.Core.ConfigurationModel;
using Servinte.Framework.Clinic.BasicInformation.Infraestructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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

        /// <summary>
        /// Descripcion relevante del uso del recurso
        /// </summary>
        /// <returns>Entidad o Informacion a Retornar</returns>
        /// <response code="200">Entidad o Informacion que acompaña, el codigo HTTP</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<Patient>), 200)]
        public IActionResult Get()
        {
            var patient= this.context.Patients.FirstOrDefault();
            
            return Ok(patient);
        }

        /// <summary>
        /// Descripcion Relevante del uso del recurso
        /// </summary>
        /// <param name="id">Descripcion de parametro1  a N</param>
        /// <returns>Entidad o Informacion a Retornar</returns>
        /// <response code="200">Descripcion Respuesta</response>
        /// <response code="404">Recurso encontrado para el ID</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Patient),200)]
        [ProducesResponseType(typeof(List<Patient>),404)]
        public async Task<IActionResult> GetById(int id)
        {
            var patient = await this.context.Patients.FirstOrDefaultAsync(c=>c.Id==id);

            if (patient == null)
                return NotFound(new { message = "Recurso no encotrado" });

            return Ok(patient);
        }

        /// <summary>
        /// Descripcion relevante del uso del recurso
        /// </summary>
        /// <param name="patient"></param>
        /// <returns>Entidad o Informacion a Retornar</returns>
        /// <response code="201">Patient creado para el request</response>
        [HttpPost]
        [ProducesResponseType(typeof(Patient),201)]
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
            this.rabbitMQBrokerClient.SendMessage<Patient,object>(patient,null ,publisher);

            return CreatedAtRoute(new { id = patient.Id }, patient);
        }

        /// <summary>
        /// Descripcion relevante del uso del recurso
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patient"></param>
        /// <returns>Entidad o Informacion a Retornar</returns>
        /// <response code="204">No Content</response>
        /// <response code="400">Request no coincidente</response>
        /// <response code="404">Recurso encontrado para el ID</response>
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult Put(int id, [FromBody]Patient patient)
        {
            if (id != patient.Id)
                return BadRequest(new { message = "Id no coincide con el recurso solicitado para actualizar" });

            var existingProduct = this.context.Patients.SingleOrDefault(p => p.Id == patient.Id);

            if (existingProduct==null)
                return NotFound(new { message = "Recurso no encotrado" });

            return NoContent();
        }

        /// <summary>
        /// Descripcion relevante del uso del recurso
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Entidad o Informacion a Retornar</returns>
        /// <response code="204">No Content</response>        
        /// <response code="404">Recurso encontrado para el ID</response>
        [HttpDelete]        
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult Delete(int id)
        {
            var patient = this.context.Patients.SingleOrDefault(p => p.Id == id);

            if (patient != null)
                this.context.Patients.Remove(patient);
            else
                return NotFound(new { message = "Recurso no encotrado" });

            return NoContent();
        }

    }
}