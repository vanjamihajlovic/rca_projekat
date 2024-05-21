using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdministratorService_WebRole.Models
{
	public class Report
	{
		public string ServiceName { get; set; }		// Reddit ili Notification
		public string ServiceStatus { get; set; }	// OK ili NOT_OK
		public DateTime Time { get; set; }
	}
}
