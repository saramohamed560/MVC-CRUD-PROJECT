using Demo.DAL.Models;
using System.Net;
using System.Net.Mail;

namespace Demo.PL.Helper
{
	public static  class EmailSettings
	{
		public static void SendEmail(Email email)
		{
			//using google smtp settings
			var client = new SmtpClient("smtp.gmail.com", 587);
			client.EnableSsl = true; //Make Email Encrypted
			client.Credentials = new NetworkCredential("saramohamed3738@gmail.com", "rlkcojfutxbghfyl");
			client.Send("saramohamed3738@gmail.com",email.Recipients,email.Subject,email.Body);	

		}
	}
}
