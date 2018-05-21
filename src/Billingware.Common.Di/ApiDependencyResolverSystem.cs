using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace Billingware.Common.Di
{
    public class ApiDependencyResolverSystem
    {
        private static Container _container;

        private static volatile bool _started;

        public static void Start()
        {
            if (_started) { return; }

            _started = true;
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();



            _container.Verify(VerificationOption.VerifyOnly);

        }

        public static Container GetContainer()
        {
            return _container;
        }

        public static TObject GetInstance<TObject>() where TObject : class => _container.GetInstance<TObject>();
    }
}
