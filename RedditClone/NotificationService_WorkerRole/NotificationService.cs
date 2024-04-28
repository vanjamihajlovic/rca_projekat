using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService_WorkerRole
{
	public class NotificationService
	{
		// Da li njemu uopšte treba servis?
		// Nema interne EP, nema eksterne EP, koristi samo queue
		// Potreban je samo HealthCheck servis, ali on već postoji

		/*
		private ServiceHost serviceHost;
		private string endPointName = "NotificaionInternal";

		public NotificationService()
		{
			RoleInstanceEndpoint inputEndPoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints[endPointName];
			string endpoint = string.Format("net.tcp://{0}/{1}", inputEndPoint.IPEndpoint, endPointName);

			serviceHost = new ServiceHost(typeof(HealthCheckServiceProvider));
			NetTcpBinding binding = new NetTcpBinding();
			serviceHost.AddServiceEndpoint(typeof(INotificationService), binding, endpoint);
		}

		public void Open()
		{
			try
			{
				serviceHost.Open();
				Trace.TraceInformation(string.Format("Host for {0} endpoint type opened successfully at {1}", endPointName, DateTime.Now));
			}
			catch (Exception e)
			{
				Trace.TraceInformation("Host open error for {0} endpoint type. Error message is: {1}. ", endPointName, e.Message);
			}
		}

		public void Close()
		{
			try
			{
				serviceHost.Close();
				Trace.TraceInformation(string.Format("Host for {0} endpoint type closed successfully at {1}", endPointName, DateTime.Now));
			}
			catch (Exception e)
			{
				Trace.TraceInformation("Host close error for {0} endpoint type. Error message is: {1}. ", endPointName, e.Message);
			}

		}
		*/
	}
}
