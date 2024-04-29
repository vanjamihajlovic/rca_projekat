using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

/*
Prilikom postavljanja novog komentara na temu - RedditService rola šalje poruku u red (notifications) gde ubacuje ID komentara

Po mogućnošću koristiti neki servis za slanje mejlova ili slati ručno (SMTP). 
Primer sistema za slanje mejlova jesu Postmark ili SendGrid.
*/

// TODO kasnije dodati UI za korisnike (kao poseban projekat)

namespace NotificationService_WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

		private HealthCheckService hcs = new HealthCheckService();
		private NotificationService ns = new NotificationService();

		public override void Run()
        {
            Trace.TraceInformation("NotificationService_WorkerRole is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Use TLS 1.2 for Service Bus connections
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

			hcs.Open();
			ns.Open();

			Trace.TraceInformation("NotificationService_WorkerRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("NotificationService_WorkerRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

			hcs.Close();
			ns.Close();

            Trace.TraceInformation("NotificationService_WorkerRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(10000);
            }
        }
    }
}
