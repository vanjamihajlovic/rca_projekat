using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdministratorService_WebRole.Models
{
	public class Email
	{
		string emailAddress;

		public string EmailAddress { get => emailAddress; set => emailAddress = value; }

		public Email() { }
	}
}