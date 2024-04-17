using Microsoft.Extensions.DependencyInjection;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Util;
using TheTechIdea.Beep.Helpers;

namespace TheTechIdea.Beep.Container
{
    public static class RegisterBeepinServiceCollection
    {
        public static IServiceCollection Services { get; private set; }

        private static IBeepService beepService;
        private static string BeepDataPath;
        public static IServiceCollection RegisterBeep(this IServiceCollection services, string directorypath, string containername, BeepConfigType configType, bool AddasSingleton = true)
        {
            Services = services;
            beepService = new   BeepService(services,directorypath, containername, configType, AddasSingleton );
            Services.AddSingleton<IBeepService>(beepService);
            ContainerMisc.CreateMainFolder();
            return Services;
        }
        public static IServiceCollection RegisterScopedBeep(this IServiceCollection services)
        {
            Services = services;
            Services.AddScoped<IBeepService>();
            return Services;
        }
        public static IServiceCollection CreateBeepMapping(this IBeepService beepService)
        {
            if(beepService!=null) {
                ContainerMisc.AddAllConnectionConfigurations(beepService);
                ContainerMisc.AddAllDataSourceMappings(beepService);
                ContainerMisc.AddAllDataSourceQueryConfigurations(beepService);
            }
            return Services;
        }
        public static IServiceCollection CreateMainFolder()
        {
            ContainerMisc.CreateMainFolder();
            return Services;
        }
   
       
    }
}
