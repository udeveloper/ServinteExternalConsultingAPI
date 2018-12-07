using System;
using System.Collections.Generic;
using System.Text;

namespace Servinte.Framework.Clinic.BasicInformation.Infraestructure
{
    public class ConfigurationPublisher
    {
        public int Id { get; set; }
        public string Applicacion { get; set; }

        public string Module { get; set; }

        public string Action { get; set; }

        public string ExchageName { get; set; }

        public string ExchangeType { get; set; }
        
        public string QueueName { get; set; }

        public string KeyRouting { get; set; }

        public string KeyBinding { get; set; }
    }
}
