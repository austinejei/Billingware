using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Billingware.Modules.Common;

namespace Billingware.Modules.Core
{
    public class CoreModule : IBillingwareModule
    {
        public void Start()
        {
            CoreActorSystem.Start();
        }

        public void Stop()
        {
            CoreActorSystem.Stop();
        }
    }
}
