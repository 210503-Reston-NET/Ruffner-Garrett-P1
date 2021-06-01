using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using StoreModels;

namespace Service
{
    public class EmailSender :  IEmailSender
    {
        private readonly SmtpClient _smtp;
         private MailAddress originEmail = new MailAddress("ddaydevtime@gmail.com"); 
         public EmailSender(SmtpClient smtp){
            _smtp = smtp;
         }

        // public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(subject, message, email);
        }

        public Task Execute( string subject, string message, string email)
        {
            var msg = new MailMessage(originEmail.Address,email, subject,message );
            // {
            //     From = new EmailAddress("Joe@contoso.com", Options.SendGridUser),
            //     Subject = subject,
            //     PlainTextContent = message,
            //     HtmlContent = message
            // };
            //msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            //msg.SetClickTracking(false, false);

            _smtp.SendAsync(msg, email);
            return null;
        }
       
    }
}