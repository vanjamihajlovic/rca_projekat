using Contracts;
using Helpers;
using Microsoft.WindowsAzure.Storage.Queue;
using ServiceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableRepository;

// Metode: PosaljiMejl, ProcitajIzReda, DobaviTekstKomentara, IspisiIzvestaj
// Iz reda (notifications) dobija ID komentara i iz tabele čita mejlove svih korisnika koji su pretplaćeni na temu
// Izveštaj o svakoj poslatoj grupi notifikacija čuvati kao |datum-vreme|id-komentara|broj-poslatih-mejlova|
// Izveštaj se čuva u posebnoj tabeli NotificationServiceTable (vrv, nije ništa u tekstu napisano)

// TODO: napravi tabelu, skontaj šta da koristiš za mailing service (postmark, sendgrid)
// Kako da dobije adrese administratora kojima treba da šalje mejl?

namespace NotificationService_WorkerRole
{ 
    public class NotificationServiceProvider : INotificationService
    {
		CloudQueue queueComments = QueueHelper.GetQueueReference("CommentNotificationsQueue");
		CloudQueue queueAdmins = QueueHelper.GetQueueReference("AdminNotificationsQueue");

		public void PosaljiMejl()
		{
			throw new NotImplementedException();
		}

		#region Komentari
		private void ProveriQueueKomentari()
		{
			if (queueComments.ApproximateMessageCount > 0)
			{
				string idKomentara = queueComments.GetMessage().AsString;

				TableRepositoryKomentar trk = new TableRepositoryKomentar();
				Komentar k = trk.DobaviKomentar(idKomentara);

				TableRepositoryTema trt = new TableRepositoryTema();
				Tema t = trt.DobaviTemu(k.IdTeme.ToString());

				string tekstKomentara = k.Sadrzaj;
				string autorKomentara = k.Autor;
				int brojMejlova = t.PretplaceniKorisnici.Count;
				DateTime vreme = DateTime.Now;

				List<int> pretplaceni = t.PretplaceniKorisnici;
				foreach (int i in pretplaceni)
				{
					PosaljiMejl();
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
