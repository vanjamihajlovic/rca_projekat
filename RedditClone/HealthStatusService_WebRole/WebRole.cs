using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Linq;

/*
Zasebna Web Role aplikacija koja koja treba da ima vizuelni prikaz dostupnosti RedditService servisa,
čita informacije iz HealthCheck table u poslednijh sat vremena 
i prikazuje procentualno uptime u poslednjih 24h
*/

namespace HealthStatusService_WebRole
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
