using Salud.Framework.Broker.Core.ConfigurationModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salud.Framework.Broker.Core
{
    public interface IBrokerClient
    {

        bool SendMessage<T,V>(T message, V configurationMessage,ConfigurationPublisherClient configurationPublisher);
    }
}
