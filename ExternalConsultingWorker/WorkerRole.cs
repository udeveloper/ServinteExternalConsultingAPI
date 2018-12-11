using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Servinte.Framework.Broker.Consumer.RabbitMQ;

namespace ExternalConsultingWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        RabbitMQConsumer client;
        public override void Run()
        {
            Trace.TraceInformation("ExternalConsultingWorker is running");

            try
            {

                this.client.ProcessMessages();
                //this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            
            client = new RabbitMQConsumer(CloudConfigurationManager.GetSetting("monitoringBrokerExchangeName"),
                    CloudConfigurationManager.GetSetting("monitoringBrokerQueueName"));

            client.CreateConnection();

            Trace.TraceInformation("ExternalConsultingWorker has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("ExternalConsultingWorker is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            this.client.Close();

            Trace.TraceInformation("ExternalConsultingWorker has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                client.ProcessMessages();
               
            }
        }
    }
}
