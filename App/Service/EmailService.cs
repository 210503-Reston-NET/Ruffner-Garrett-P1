using System;
using System.ComponentModel;
using System.Net.Mail;
using StoreModels;
using Serilog;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace Service
{
    public class EmailService : IEmailService
    {
         private SmtpClient _smtp;
         private MailAddress originEmail = new MailAddress("ddaydevtime@gmail.com"); 
         public EmailService(SmtpClient smtp){
            _smtp = smtp;
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
            Log.Debug("Sending Welcome Email to: {0}",customer.Email);
            string subject = String.Format("Hello {0}, Welcome to watch shop!", customer.Name);
            string body = String.Format("We have the following info.\nName: {0} \nAddress: {1} \nEmail: {2}", customer.Name, customer.Address, customer.Email);
            MailMessage mm = new MailMessage(originEmail.Address, customer.Email, subject, body);
            _smtp.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
            string userState = customer.Email;
            _smtp.SendAsync(mm, userState);
            // _smtp.Send(mm);
        }
        public void SendOrderConfirmationEmail(ApplicationUser customer, Order order)
        {   
            Log.Debug("Sending Order Confirmation Email to: {0}",customer.Email);
            MailAddress from = new MailAddress("ddaydevtime@gmail.com");
            string subject = String.Format("Thank you for your order from The Watch Shop!", customer.Name);
            StringBuilder sb = new StringBuilder("", 500);
            sb.AppendFormat("{0},\nHere are your order details:\n",customer.Name);
            sb.AppendFormat("Order from: {0}\n{1}\n",order.Location.LocationName,order.Location.Address);
            sb.AppendFormat("Shipping Address: {0}\n",customer.Address);
            sb.AppendFormat("Order Items:\n\n");
            // foreach (Item item in order.Items)
            // {
            //     sb.AppendFormat("{0} x {1}\n",item.Product, item.Quantity);
            // }
            sb.AppendFormat("\nOrder Total: ${0}\n",order.Total);
            string body = sb.ToString();
            MailMessage mm = new MailMessage(from.Address, customer.Email, subject, body);
            _smtp.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
            string userState = customer.Email;
            _smtp.SendAsync(mm, userState);
        }           
    }
}