using Microsoft.AspNetCore.Identity.UI.Services;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class EmailSender : IEmailSender
    {
		//private readonly EmailOptions _emailOptions;

		//public EmailSender(EmailOptions emailOptions)
		//{
		//	_emailOptions = emailOptions;
		//}

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
			return SendSimpleMessage(htmlMessage, subject, email);

		}

		public Task SendSimpleMessage(string message, string subject, string email)
		{
			RestClient client = new RestClient();
			client.BaseUrl = new Uri("https://api.mailgun.net/v3");
			client.Authenticator =
			new HttpBasicAuthenticator("api",
										"7497ce7f6ba7232db75ffda98a869ecd-e5e67e3e-bcd66e5d"
									   );
			RestRequest request = new RestRequest();
			request.AddParameter("domain", "sandboxc96d9c12c1d447cb8a987cc7d2b0d414.mailgun.org", ParameterType.UrlSegment);
			request.Resource = "{domain}/messages";
			request.AddParameter("from", "Mailgun Sandbox <postmaster@sandboxc96d9c12c1d447cb8a987cc7d2b0d414.mailgun.org>");
			request.AddParameter("to", $"Bulky Book user <{email}>");
			request.AddParameter("subject", subject);
			request.AddParameter("text", message);
			request.Method = Method.POST;
			return client.ExecuteAsync(request);
		}
	}
}
