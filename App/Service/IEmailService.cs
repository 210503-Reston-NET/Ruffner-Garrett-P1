using StoreModels;

namespace Service
{
    public interface IEmailService
    {
        void SendWelcomeEmail(Customer customer);
        void SendOrderConfirmationEmail(Customer customer, Order order);
    }
}