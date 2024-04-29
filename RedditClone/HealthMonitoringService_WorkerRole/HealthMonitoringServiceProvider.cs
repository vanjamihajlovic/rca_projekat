﻿using Contracts;
using Helpers;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

// Metode PosaljiZahtev, NapisiIzvestaj
// Izveštaj se čuva u posebnoj tabeli HealthCheckTable, |vreme-datum|poruka (OK/NOT_OK)|

// HealthCheck se vrši svakih 1-5 sekundo

namespace HealthMonitoringService_WorkerRole
{
    public class HealthMonitoringServiceProvider : IHealthCheck
    {
        CloudQueue queue = QueueHelper.GetQueueReference("AdminNotificationsQueue");
        
        private static IHealthCheck proxy1;
        private static IHealthCheck proxy2;

        static NetTcpBinding binding = new NetTcpBinding();
        private static string internalEndpointName = "HealthCheck";
        
        public bool HealthCheck()
        {
            PerformCheckReddit();
            PerformCheckNotifications();

            // TODO promeni
            return true;
        }

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
            for (int i = 0; i < numOfInstances; i++)
            {
                proxy1 = new ChannelFactory<IHealthCheck>(binding, redditEndpoints[i]).CreateChannel();
                bool alive = proxy1.HealthCheck();
                if (!alive) sveOkej = false;
            }

			// Napravi izveštaj i upiši ga u tabelu

            if (!sveOkej)
            {
				// Dodaj idIzvestaja u AdminNotificaionQueue
				// queue.AddMessage(new CloudQueueMessage(idProvere));
			}
		}

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
            for (int i = 0; i < numOfInstances; i++)
            {
                proxy2 = new ChannelFactory<IHealthCheck>(binding, notificationEndpoints[i]).CreateChannel();
                bool alive = proxy2.HealthCheck();
                if (!alive) sveOkej = false;
            }

			// Napravi izveštaj i upiši ga u tabelu

			if (!sveOkej)
            {
				// Dodaj idIzvestaja u AdminNotificaionQueue
				// queue.AddMessage(new CloudQueueMessage(idProvere));
			}
		}
    }
}
