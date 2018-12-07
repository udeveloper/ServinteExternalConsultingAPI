using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Servinte.Framework.Clinic.BasicInformation.Infraestructure
{
    public class ExternalConsultingContext:DbContext
    {
        public ExternalConsultingContext(DbContextOptions<ExternalConsultingContext> options):base(options)
        {

        }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<ConfigurationPublisher> ConfigurationPublishers { get; set; }
    }

}
