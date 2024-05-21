using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AdministratorService_WebRole
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
		{
			return base.OnStart();
        }
    }
}
