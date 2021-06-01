using System;
using System.ComponentModel;
using StoreModels;
using Serilog;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using System.Net.Mail;
using System.Net;

namespace Service
{
    public class EmailService : IEmailSender
    {
        private readonly SmtpClient _smtp;
        //  private MailAddress originEmail = new MailAddress("ddaydevtime@gmail.com"); 
        //  public EmailService(SmtpClient smtp){
        //     _smtp = smtp;
        //  }
        private readonly EmailSettings _emailSettings;
        private readonly IHostingEnvironment _env;

        public EmailService(
            IOptions<EmailSettings> emailSettings,
            IHostingEnvironment env)
        {
            _emailSettings = emailSettings.Value;
            _env = env;
        }


        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
             String token = (string) e.UserState;

            if (e.Error != null)
            {
                Log.Error("Message to [{0}] Failed. {1}", token, e.Error.ToString());
            } else
            {
                Log.Information("Message to {0} sent successfully.", token);
            }
        }
        public void SendWelcomeEmail(ApplicationUser customer)
        {
            var smtpClient = new SmtpClient(_emailSettings.Host)
            {
                Port = _emailSettings.Port,
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = true,
            };
            Log.Debug("Sending Welcome Email to: {0}",customer.Email);
            string subject = String.Format("Hello {0}, Welcome to watch shop!", customer.Name);
            string body = String.Format("We have the following info.\nName: {0} \nAddress: {1} \nEmail: {2}", customer.Name, customer.Address, customer.Email);
            MailMessage mm = new MailMessage(_emailSettings.Host+"@gmail.com", customer.Email, subject, body);
            smtpClient.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
            string userState = customer.Email;
            smtpClient.SendAsync(mm, userState);
            // _smtp.Send(mm);
        }
        public void SendOrderConfirmationEmail(ApplicationUser customer, Order order)
        {   
            var smtpClient = new SmtpClient(_emailSettings.Host)
            {
                Port = _emailSettings.Port,
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = true,
            };
            Log.Debug("Sending Order Confirmation Email to: {0}",customer.Email);
            MailAddress from = new MailAddress("ddaydevtime@gmail.com");
            string subject = String.Format("Thank you for your order from The Watch Shop!", customer.Name);
            StringBuilder sb = new StringBuilder("", 500);
            sb.AppendFormat("{0},\nHere are your order details:\n",customer.Name);
            sb.AppendFormat("Order from: {0}\n{1}\n",order.Location.LocationName,order.Location.Address);
            sb.AppendFormat("Shipping Address: {0}\n",customer.Address);
            sb.AppendFormat("Order Items:\n\n");
            foreach (OrderItem item in order.OrderItems)
            {
                sb.AppendFormat("{0} x {1}\n",item.Product, item.Quantity);
            }
            sb.AppendFormat("\nOrder Total: ${0}\n",order.Total);
            string body = sb.ToString();
            MailMessage mm = new MailMessage(from.Address, customer.Email, subject, body);
            smtpClient.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
            string userState = customer.Email;
            smtpClient.SendAsync(mm, userState);
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var smtpClient = new SmtpClient(_emailSettings.Host)
            {
                Port = _emailSettings.Port,
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = true,
            };

            Log.Verbose("Email sent from async");
            string from = _emailSettings.Username+"@gmail.com";
            MailMessage mm = new MailMessage(from, email);
            mm.Subject = subject; 
            mm.Body = message; 
            mm.IsBodyHtml = true;

            return  smtpClient.SendMailAsync(mm);
            
        }
    }
}