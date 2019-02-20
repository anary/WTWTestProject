using CPR.Business;
using CPR.Business.Services;
using CPR.Core.Interfaces;
using CPR.IO.Services;
using Unity;

namespace ReportorConsole.Container
{
    public class Container
    {
        public static UnityContainer Register()
        {
            var container = new UnityContainer();
            container.RegisterType<ITextFileService, TextFileService>(new PerResolveLifetimeManager());
            container.RegisterType<IIndexServices, IndexServices>();
            container.RegisterType<IClaimsDataProcessingService, ClaimsDataProcessService>();
            container.RegisterType<IClaimsValueProcessService, ClaimsValueProcessService>();
            container.RegisterType<IClaimPaymentReport, ClaimPaymentReport>();
            return container;
        }
    }
}
