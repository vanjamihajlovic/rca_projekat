﻿using Microsoft.WindowsAzure.ServiceRuntime;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

/*
Na svakih 1-5 sekundi šalje zahtev ka RedditService i NotificationService /health-monitoring endpoint,
gde ukoliko sam zahtev prođe smatra se da je sve kako treba, a ukoliko ne prođe šalje se mejl 
na mejl adrese koje je moguće urediti kroz konzolnu aplikaciju (implementirati autentifikaciju nekog tipa)

Svejedno da li je zahtev prošao ili nije, neophodno je upisati poruku u posebnu tabelu HealthCheck 
sa trenutnim datumom i vremenom i statusom da li je uspelo ili nije
npr. 2023-11-03:12:23.33.3333_OK ili 2023-11-03:12:23.33.3333_NOT_OK
*/

namespace HealthMonitoringService_WorkerRole
{
	public class WorkerRole : RoleEntryPoint
	{
		private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

		private HealthMonitoring hf = new HealthMonitoring();

		public override void Run()
		{
			Trace.TraceInformation("HealthMonitoringService_WorkerRole is running");

			while (true)
			{
				hf.HealthCheck();

				Thread.Sleep(5000);
				Trace.TraceInformation("Working", "Information");
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

			Trace.TraceInformation("HealthMonitoringService_WorkerRole has been started");

			return result;
		}

		public override void OnStop()
		{
			Trace.TraceInformation("HealthMonitoringService_WorkerRole is stopping");

			this.cancellationTokenSource.Cancel();
			this.runCompleteEvent.WaitOne();

			base.OnStop();

			Trace.TraceInformation("HealthMonitoringService_WorkerRole has stopped");
		}

		//private async Task RunAsync(CancellationToken cancellationToken)
		//{
		//    while (!cancellationToken.IsCancellationRequested)
		//    {
		//        Trace.TraceInformation("Working");
		//        await Task.Delay(10000);
		//    }
		//}
	}
}
