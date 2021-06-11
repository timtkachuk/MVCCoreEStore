using PaymentBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IyzicoPaymentModule
{
    class Module : Payment
    {
        public override string ProviderName => "Iyzico";

        public override PaymentResult Pay(decimal Amount, string CardNumber, string Name, string ExpireDate, string CVC, int? Installment = 1)
        {
            return new PaymentResult { Succeded = true, Error = null };
        }

        public override PaymentResult Pay(PaymentParameters parameters)
        {
            return new PaymentResult { Succeded = true, Error = null };
        }
    }
}
