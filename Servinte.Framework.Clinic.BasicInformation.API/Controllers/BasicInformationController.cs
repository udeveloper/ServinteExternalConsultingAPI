using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Servinte.Framework.Clinic.BasicInformation.Infraestructure;

namespace Servinte.Framework.Clinic.BasicInformation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController()]
    public class BasicInformationController : Controller
    {
        private readonly ExternalConsultingContext context;

        public BasicInformationController(ExternalConsultingContext context)
        {
            this.context = context;
        }

        // GET: BasicInformation
        [HttpGet]
        public IActionResult Get()
        {
            var patient= this.context.Patients.SingleOrDefault();
            return Ok(patient);
        }

    }
}