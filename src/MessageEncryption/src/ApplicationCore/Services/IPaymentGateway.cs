using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Services
{
    public interface IPaymentGateway
    {
        Task ProcessCreditCard(string number, string expiry, string cvv);
    }

    public class PaymentGateway : IPaymentGateway
    {
        public Task ProcessCreditCard(string number, string expiry, string cvv)
        {
            return Task.CompletedTask;
        }
    }
}