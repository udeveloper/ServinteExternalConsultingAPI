using System;
using System.Collections.Generic;
using System.Text;

namespace Salud.Framework.Broker.Core.ConfigurationModel
{
    public class ConfigurationPublisherServer
    {
        public string ExchangeType { get; set; }

        public string ExchageName { get; set; }

        public Queue QueueConfiguration { get; set; }

        public string KeyBinding { get; set; }
    }
}
