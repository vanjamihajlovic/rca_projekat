using Contracts;
using Helpers;
using Microsoft.WindowsAzure.Storage.Queue;
using SendGrid;
using SendGrid.Helpers.Mail;
using ServiceData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TableRepository;

// Metode: PosaljiMejl, ProcitajIzReda, DobaviTekstKomentara, IspisiIzvestaj
// Iz reda (notifications) dobija ID komentara i iz tabele čita mejlove svih korisnika koji su pretplaćeni na temu
// Izveštaj o svakoj poslatoj grupi notifikacija čuvati kao |datum-vreme|id-komentara|broj-poslatih-mejlova|
// Izveštaj se čuva u posebnoj tabeli NotificationServiceTable (vrv, nije ništa u tekstu napisano)

// TODO: napravi tabelu, skontaj šta da koristiš za mailing service (postmark, sendgrid)
// Kako da dobije adrese administratora kojima treba da šalje mejl?


//mejl cloudproj@gmail.com
//sifra cloudproj123


namespace NotificationService_WorkerRole
{
    public class NotificationServiceProvider : INotificationService
    {
        CloudQueue queueComments = QueueHelper.GetQueueReference("CommentNotificationsQueue");
        CloudQueue queueAdmins = QueueHelper.GetQueueReference("AdminNotificationsQueue");


        public async Task PosaljiMejl(string korisnikEmail, string tekstKomentara, string autorKomentara, DateTime vreme, string naslovTeme)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("cloudprojekat@gmail.com", "Example User");
            var subject = "Novi komentar za temu : " + naslovTeme;
            var to = new EmailAddress(korisnikEmail, "Example User");
            var plainTextContent = autorKomentara + " commented: " + tekstKomentara;
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            //Console.WriteLine(response.StatusCode);
            //Console.WriteLine(response.Body.ReadAsStringAsync());
        }


        // async Task INotificationService.PosaljiMejl(string korisnikEmail, string tekstKomentara, string autorKomentara, DateTime vreme)
        //{
        //            var apiKey = "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";
        //            var client = new SendGridClient(apiKey);
        //            var from = new EmailAddress("cloudprojekat@gmail.com", "Example User");
        //            var subject = "Sending with SendGrid is Fun";
        //            var to = new EmailAddress("mihajlovicavanja@gmail.com", "Example User");
        //            var plainTextContent = "and easy to do anywhere, even with C#";
        //            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
        //            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        //            var response = await client.SendEmailAsync(msg);
        //            Console.WriteLine(response.StatusCode);
        //            Console.WriteLine(response.Body.ReadAsStringAsync());
        //}



        #region Komentari
        private async Task ProveriQueueKomentari()
        {
            if (queueComments.ApproximateMessageCount > 0)
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
    }
}
