using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billingware.Modules.Common
{
    public interface IBillingwareModule
    {
        void Start();

        void Stop();
    }
}
