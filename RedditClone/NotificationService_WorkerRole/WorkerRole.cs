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

Po mogućnošću koristiti neki servis za slanje mejlova ili slati ručno (SMTP). 
Primer sistema za slanje mejlova jesu Postmark ili SendGrid.
*/

// TODO kasnije dodati UI za korisnike (kao poseban projekat)



//mejl cloudproj@gmail.com
//sifra cloudproj123
//za sendGrid nalog je drugo



namespace NotificationService_WorkerRole
{

    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private HealthCheckService hcs = new HealthCheckService();

        //ovo nam valjda vise nece trebati
        // private NotificationService ns = new NotificationService();

        private readonly QueueHelper queueHelper;

        CloudQueue queueComments = QueueHelper.GetQueueReference("CommentNotificationsQueue");
        CloudQueue queueAdmins = QueueHelper.GetQueueReference("AdminNotificationsQueue");


        public async Task PosaljiMejl(string korisnikEmail, string tekstKomentara, string autorKomentara, DateTime vreme, string naslovTeme)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("cloudprojekat@gmail.com", "CLOUD Projekat Mejl");
            var subject = "Novi komentar za temu : " + naslovTeme;
            var to = new EmailAddress(korisnikEmail, "Example User");
            var plainTextContent = autorKomentara + " commented: " + tekstKomentara;
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            //Console.WriteLine(response.StatusCode);
            //Console.WriteLine(response.Body.ReadAsStringAsync());
        }

        #region Komentari
        private async Task ProveriQueueKomentari()
        {
            if (queueComments.ApproximateMessageCount > 0)
            {
                var message = queueComments.GetMessage();

                if (message != null)
                {
                    string idKomentara = queueComments.GetMessage().AsString;

                    TableRepositoryKomentar trk = new TableRepositoryKomentar();
                    Komentar k = trk.DobaviKomentar(idKomentara);

                    TableRepositoryTema trt = new TableRepositoryTema();
                    Tema t = trt.DobaviTemu(k.IdTeme.ToString());


                    TableRepositorySubscribe trs = new TableRepositorySubscribe();
                    List<Subscribe> pretplaceni = trs.DobaviSvePrijavljene(t.Id.ToString());


                    string tekstKomentara = k.Sadrzaj;
                    string autorKomentara = k.Autor;
                    string naslovTeme = t.Naslov;
                    int brojMejlova = t.PretplaceniKorisnici.Count;
                    DateTime vreme = DateTime.Now;


                    foreach (Subscribe i in pretplaceni)
                    {
                        //TableRepositoryKorisnik trkor = new TableRepositoryKorisnik();
                        //Korisnik kor = trkor.DobaviKorisnika(pretplaceni.ToString());
                        await PosaljiMejl(i.UserId, tekstKomentara, autorKomentara, vreme, naslovTeme);
                    }

                    UpisiNotifikacijuUTabelu(k.Id, vreme, brojMejlova);
                    // Delete the message after processing
                    queueComments.DeleteMessage(message.Id, message.PopReceipt);
                }
            }
        }

        private void UpisiNotifikacijuUTabelu(int idKomentara, DateTime vreme, int brojPoslatihMejlova)
        {
            Notifikacija n = new Notifikacija(idKomentara, vreme, brojPoslatihMejlova);
            TableRepositoryNotifikacije trn = new TableRepositoryNotifikacije();
            trn.SacuvajNotifikaciju(n);
        }
        #endregion

        #region Provere
        private void ProveriQueueProvere()
        {
            if (queueComments.ApproximateMessageCount > 0)
            {
                string idIzvestaja = queueAdmins.GetMessage().AsString;

                TableRepositoryIzvestaj tri = new TableRepositoryIzvestaj();
                Izvestaj i = tri.DobaviIzvestaj(idIzvestaja);

                string poruka = i.Sadrzaj;
                DateTime vreme = i.Vreme;

                // TODO dalju logiku
            }
        }

        private void UpisiIzvwstajUTabelu(int idIzvestaja, DateTime vreme, string poruka)
        {
            Izvestaj i = new Izvestaj(idIzvestaja, vreme, poruka);
            TableRepositoryIzvestaj tri = new TableRepositoryIzvestaj();
            tri.SacuvajIzvestaj(i);
        }


        #endregion


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
                //ovo vrv moze i da se obrise

                Trace.TraceInformation("Working");

                // Call ProveriQueueKomentari to process messages from the queue
                await ProveriQueueKomentari();

                // Wait for a period before checking the queue again
                await Task.Delay(10000, cancellationToken);
            }

        }
    }



    }

