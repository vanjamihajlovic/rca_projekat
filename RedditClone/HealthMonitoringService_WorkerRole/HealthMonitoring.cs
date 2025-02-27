﻿using Contracts;
using Helpers;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage.Queue;
using ServiceData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TableRepository;

namespace HealthMonitoringService_WorkerRole
{
    public  class HealthMonitoring
    {
        private static CloudQueue queue = QueueHelper.GetQueueReference("AdminNotificationsQueue");

        private static IHealthCheck proxy1;
        private static IHealthCheck proxy2;

        static NetTcpBinding binding = new NetTcpBinding();
        private static string internalEndpointName = "HealthCheck";

        #region Main Function 
        public void HealthCheck()
        {
            PerformCheckReddit();
            PerformCheckNotifications();
        }
        #endregion
		
        #region Reddit
        private void PerformCheckReddit()
        {
            binding.TransactionFlow = true;
            Trace.WriteLine(String.Format("Perfoming Health Check on RedditService"));

            // Interni EP za RedditService
            List<EndpointAddress> redditEndpoints = RoleEnvironment.Roles["RedditService_WebRole"].
                Instances.Select(process => new EndpointAddress(String.Format("net.tcp://{0}/{1}",
                process.InstanceEndpoints[internalEndpointName].IPEndpoint.ToString(), internalEndpointName))).ToList();

            int numOfInstances = redditEndpoints.Count;

            bool sveOkej = true;
			bool alive = false;
            for (int i = 0; i < numOfInstances; i++)
            {
				try
				{
					proxy1 = new ChannelFactory<IHealthCheck>(binding, redditEndpoints[i]).CreateChannel();
					alive = proxy1.HealthCheck();
				}
				catch (Exception e)
				{
					Trace.WriteLine(e.Message);
				}
                
				if (!alive)
				{
					Trace.WriteLine($"Reddit_Service {i} is unavailable.");
					sveOkej = false;
				}

				Trace.WriteLine($"Reddit_Service {i} is available.");
			}

			// Cuvanje izvestaja u tabelu
            int idIzvestaja = SacuvajIzvestajRedit(sveOkej);

            if (!sveOkej)
            {
                // Dodaj idIzvestaja u AdminNotificaionQueue
                queue.AddMessage(new CloudQueueMessage(idIzvestaja.ToString()));
            }
        }

        private int SacuvajIzvestajRedit(bool sveOkej)
        {
            // Napravi izveštaj
            string poruka = (sveOkej) ? "RedditService OK" : "RedditService NOT OK";
            DateTime vreme = DateTime.Now;
			int idIzvestaja = (int)DateTime.UtcNow.Ticks;
			Izvestaj ri = new Izvestaj(idIzvestaja, vreme, poruka);

            // Upiši izveštaj u tabelu
            TableRepositoryIzvestaj tri = new TableRepositoryIzvestaj();
            tri.SacuvajIzvestaj(ri);

            return idIzvestaja;
        }
        #endregion

        #region Notifikacije
        private void PerformCheckNotifications()
        {
            binding.TransactionFlow = true;
            Trace.WriteLine(String.Format("Perfoming Health Check on NotificationService"));

            // Interni EP za NotificationService
            List<EndpointAddress> notificationEndpoints = RoleEnvironment.Roles["NotificationService_WorkerRole"].
                Instances.Select(process => new EndpointAddress(String.Format("net.tcp://{0}/{1}",
                process.InstanceEndpoints[internalEndpointName].IPEndpoint.ToString(), internalEndpointName))).ToList();

            int numOfInstances = notificationEndpoints.Count;

            bool sveOkej = true;
			bool alive = false;
            for (int i = 0; i < numOfInstances; i++)
            {
				try
				{
					proxy2 = new ChannelFactory<IHealthCheck>(binding, notificationEndpoints[i]).CreateChannel();
					alive = proxy2.HealthCheck();
				}
				catch (Exception e)
				{
					Trace.WriteLine(e.Message);
				}
                
				if (!alive)
				{
					Trace.WriteLine($"Notification_Service {i} is unavailable.");
					sveOkej = false;
				}

				Trace.WriteLine($"Notification_Service {i} is available.");
			}

			int idIzvestaja = SacuvajIzvestajNotifikacije(sveOkej);

            if (!sveOkej)
            {
                // Dodaj idIzvestaja u AdminNotificaionQueue
                queue.AddMessage(new CloudQueueMessage(idIzvestaja.ToString()));
            }
        }

        private int SacuvajIzvestajNotifikacije(bool sveOkej)
        {
            // Napravi izveštaj
            string poruka = (sveOkej) ? "NotificationService OK" : "NotificationService NOT OK";
            DateTime vreme = DateTime.Now;
			int idIzvestaja = (int)DateTime.UtcNow.Ticks;
			Izvestaj ni = new Izvestaj(idIzvestaja, vreme, poruka);

            // Upiši izveštaj u tabelu
            TableRepositoryIzvestaj tri = new TableRepositoryIzvestaj();
            tri.SacuvajIzvestaj(ni);

            return idIzvestaja;
        }
        #endregion
    }
}
