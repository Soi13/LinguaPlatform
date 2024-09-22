using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;


namespace LinguaPlatform.Models
{
    public class EmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
             var emailMessage = new MimeMessage();

             emailMessage.From.Add(new MailboxAddress("Lingua-terra administrator", "admin@lingua-terra.com"));
             emailMessage.To.Add(new MailboxAddress(email, email));
             emailMessage.Subject = subject;
             emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
             {
                 Text = message
             };

             using (var client = new SmtpClient())
             {               
                await client.ConnectAsync("mail.nic.ru", 465, true);
                await client.AuthenticateAsync("admin@lingua-terra.com", "");
                await client.SendAsync(emailMessage);

                 await client.DisconnectAsync(true);
             }

        }
    }
}
