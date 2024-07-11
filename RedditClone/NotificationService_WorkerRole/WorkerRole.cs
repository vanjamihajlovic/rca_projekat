using Helpers;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage.Queue;
using SendGrid;
using SendGrid.Helpers.Mail;
using ServiceData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TableRepository;

/*
Prilikom postavljanja novog komentara na temu - RedditService rola šalje poruku u red (notifications) gde ubacuje ID komentara
*/


namespace NotificationService_WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private HealthCheckService hcs = new HealthCheckService();

        // Samo deklaracija ovde, a u RunAsync se dobavlja referenca
        CloudQueue queueComments;
        CloudQueue queueAdmins;

		#region WorkerRole methods
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

            Trace.TraceInformation("NotificationService_WorkerRole has stopped");
        }

        // Pošto je sam po sebi Queue asinhron, onda se u RunAsync mora raditi sa njim
        private async Task RunAsync(CancellationToken cancellationToken)
        {
            queueComments = QueueHelper.GetQueueReference("CommentNotificationsQueue");
            queueAdmins = QueueHelper.GetQueueReference("AdminNotificationsQueue");

            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");

                if (queueComments != null && queueAdmins != null)
                {
                    await ProveriQueueKomentari();
                    await ProveriQueueAdmini();
                }
                else
                {
                    Trace.TraceError("Queue initialization failed.");
                }

                // Wait for a period (10 sec) before checking the queue again
                await Task.Delay(10000, cancellationToken);
            }
        }
        #endregion WorkerRole methods

        #region PostGrid methods
        public async Task PosaljiMejl(string korisnikEmail, string tekstKomentara, string autorKomentara, DateTime vreme, string naslovTeme)
		{
			var apiKey = "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";
			var client = new SendGridClient(apiKey);
			var from = new EmailAddress("cloudprojekat@gmail.com", "CLOUD Projekat Mejl");
			var subject = "Novi komentar za temu : " + naslovTeme;
			var to = new EmailAddress(korisnikEmail, "Example User");
			var plainTextContent = autorKomentara + " commented: " + tekstKomentara;
            var htmlContent = autorKomentara + " commented: ' <strong>" + tekstKomentara + "</strong> ' .";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
			var response = await client.SendEmailAsync(msg);
			//Console.WriteLine(response.StatusCode);
			//Console.WriteLine(response.Body.ReadAsStringAsync());
		}

        public async Task PosaljiMejlAdminima(string korisnikEmail, string tekstKomentara, DateTime vreme)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("cloudprojekat@gmail.com", "CLOUD Projekat Mejl");
            var subject = "Pao servis";
            var to = new EmailAddress(korisnikEmail, "Example User");
            var plainTextContent = tekstKomentara + " " + vreme;
            var htmlContent = "<strong>Service stopped working! </strong>" + tekstKomentara + " " + vreme;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            //Console.WriteLine(response.StatusCode);
            //Console.WriteLine(response.Body.ReadAsStringAsync());
        }
        #endregion PostGrid methods

        #region NotificationService methods
        // Komentari
        private async Task ProveriQueueKomentari()
        {
            await queueComments.FetchAttributesAsync();
            if (queueComments.ApproximateMessageCount > 0)
			{
				var message = queueComments.GetMessage();

				if (message != null)
				{
					string idKomentara = message.AsString;

					TableRepositoryKomentar trk = new TableRepositoryKomentar();
					Komentar k = trk.DobaviKomentar(idKomentara);

					TableRepositoryTema trt = new TableRepositoryTema();
					Tema t = trt.DobaviTemu(k.IdTeme.ToString());

					TableRepositorySubscribe trs = new TableRepositorySubscribe();
					List<Subscribe> pretplaceni = trs.DobaviSvePrijavljene(t.Id.ToString());

					string tekstKomentara = k.Sadrzaj;
					string autorKomentara = k.Autor;
					string naslovTeme = t.Naslov;
					int brojMejlova = pretplaceni.Count;
					DateTime vreme = DateTime.Now;

					foreach (Subscribe i in pretplaceni)
					{
						await PosaljiMejl(i.UserId, tekstKomentara, autorKomentara, vreme, naslovTeme);
					}

					UpisiNotifikacijuUTabelu(idKomentara, vreme, brojMejlova);
					// Delete the message after processing
					queueComments.DeleteMessage(message.Id, message.PopReceipt);
				}
			}
		}

        private async Task ProveriQueueAdmini()
        {
            await queueAdmins.FetchAttributesAsync();
            if (queueAdmins.ApproximateMessageCount > 0)
            {
                var message = queueAdmins.GetMessage();

                if (message != null)
                {
                    string idIzvestaja = message.AsString;

                    TableRepositoryIzvestaj tri = new TableRepositoryIzvestaj();
                    Izvestaj i = tri.DobaviIzvestaj(idIzvestaja);

                    TableRepositoryAdminEmail trae = new TableRepositoryAdminEmail();
                    List<AdminEmail> aeList = trae.DobaviSveMejlove();
                   
                    string tekstIzvestaja = i.Sadrzaj;
                    int brojMejlova = aeList.Count;
                    DateTime vreme = DateTime.Now;

                    foreach (AdminEmail ae in aeList)
                    {
                        await PosaljiMejlAdminima(ae.EmailAdresa, i.Sadrzaj, i.Vreme);
                    }

                    // Nije trazeno u zadatku, ali moze se dodati
                    //UpisiNotifikacijuUTabelu(idIzvestaja, vreme, brojMejlova);
                    
                    // Delete the message after processing
                    queueAdmins.DeleteMessage(message.Id, message.PopReceipt);
                }
            }
        }
        
        private void UpisiNotifikacijuUTabelu(string idKomentara, DateTime vreme, int brojPoslatihMejlova)
		{
			Notifikacija n = new Notifikacija(idKomentara, vreme, brojPoslatihMejlova);
			TableRepositoryNotifikacije trn = new TableRepositoryNotifikacije();
			trn.SacuvajNotifikaciju(n);
		}
		#endregion NotificationService methods
	}
}
