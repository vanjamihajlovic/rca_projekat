﻿using Microsoft.WindowsAzure.ServiceRuntime;
using System.Threading;

/*
Korisnički interfejs je WebRole aplikacija koja treba da opsluži korisnika u interakciji sa forumom.
*/

namespace RedditService_WebRole
{
    public class WebRole : RoleEntryPoint
    {
        private HealthCheckService hcs = new HealthCheckService();

        public override bool OnStart()
        {
            // Pokretanje pozadinske niti za server
            Thread nit = new Thread(() =>
            {
                RedditService server = new RedditService();
                server.JobServer();
                server.Open();
            });
            nit.IsBackground = true;
            nit.Start();

            hcs.Open();

            return base.OnStart();
        }
    }
}
