using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Utility
{
    public class EmailSender : IEmailSender
    {
        //public string SendGridSecret { get; set; }
        //public EmailSender(IConfiguration _config)
        //{
        //    SendGridSecret = _config.GetValue<String>("SendGrid:SecretKey");
        //}
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {

            //var client = new SendGridClient(SendGridsecret);
            //var from = new EmailAddress("example@gmail.com", "Example User");
            //var to = new EmailAddress(email);
            //var massage = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);
            //return client.SendEmailAsync(massage);

            //lgic to send email
            return Task.CompletedTask;
        }
    }

}
