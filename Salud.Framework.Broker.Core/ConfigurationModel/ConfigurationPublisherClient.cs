using System;
using System.Collections.Generic;
using System.Text;

namespace Salud.Framework.Broker.Core.ConfigurationModel
{
    public class ConfigurationPublisherClient
    {
        public string Applicacion { get; set; }

        public string Module { get; set; }

        public string Action { get; set; }

        public string DocumentName { get; set; }

        public Dictionary<string, object> PropertiesCustom { get; set; }
    }
}
