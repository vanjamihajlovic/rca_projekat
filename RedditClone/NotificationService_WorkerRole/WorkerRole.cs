using Helpers;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

/*
Prilikom postavljanja novog komentara na temu - RedditService rola šalje poruku u red (notifications) gde ubacuje ID komentara

Po mogućnošću koristiti neki servis za slanje mejlova ili slati ručno (SMTP). 
Primer sistema za slanje mejlova jesu Postmark ili SendGrid.
*/

// TODO kasnije dodati UI za korisnike (kao poseban projekat)



//mejl cloudproj@gmail.com
//sifra cloudproj123



namespace NotificationService_WorkerRole
{
    //dvdvdv
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private HealthCheckService hcs = new HealthCheckService();

        //ovo nam valjda vise nece trebati
        // private NotificationService ns = new NotificationService();

        private readonly QueueHelper queueHelper;
        CloudQueue queueSubs = QueueHelper.GetQueueReference("RedditSubscribeTabela");

        

        

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
           // ns.Open();

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
           // ns.Close();

            Trace.TraceInformation("NotificationService_WorkerRole has stopped");
        }
		
        // Ovde u asinhronom ce ici citanje iz QUEUE
        private async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(10000);
            }
        }



    }
}
