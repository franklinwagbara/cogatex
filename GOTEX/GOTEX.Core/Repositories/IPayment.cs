using System.Collections.Generic;
using GOTEX.Core.BusinessObjects;

namespace GOTEX.Core.Repositories
{
    public interface IPayment<T> where T : class
    {
        T Insert(PaymentApproval item);
        string CreateManualPayment(Application application, ManualRemitaValue item, double amount, string feedback);
        List<PaymentApproval> GetAll();
    }
}