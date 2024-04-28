using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

// Metode PosaljiZahtev, PosaljiMejl (ako je neuspesan zahtev), NapisiIzvestaj
// Izveštaj se čuva u posebnoj tabeli HealthCheckTable, |vreme-datum|poruka (OK/NOT_OK)|

namespace HealthMonitoringService_WorkerRole
{
    public class HealthMonitoringServiceProvider : IHealthCheck
    {
        private static IHealthCheck proxy1;
        private static IHealthCheck proxy2;

        public static void Connect()
        {
            // Pronaći interne EP
        }

        public bool HealthCheck()
        {
            throw new NotImplementedException();
        }

        public void PerformCheck()
        {
            Connect();

            bool alive1 = proxy1.HealthCheck();
            bool alive2 = proxy1.HealthCheck();
            
            // Poslati mejl
            // Upisati u tabbelu vreme i poruku
        }
    }
}
