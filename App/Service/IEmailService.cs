using StoreModels;

namespace Service
{
    public interface IEmailService
    {
        void SendWelcomeEmail(ApplicationUser customer);
        void SendOrderConfirmationEmail(ApplicationUser customer, Order order);
    }
}