using PaymentBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCoreEStore.Services
{
    public interface IPaymentService
    {
        IList<IPayment> PaymentMethods { get; }

        Task<PaymentResult> Pay(PaymentParameters parameters);
    }
}
