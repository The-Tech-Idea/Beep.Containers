using Microsoft.Extensions.DependencyInjection;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Container.Shared;
using TheTechIdea.Beep.Editor;

using TheTechIdea.Beep.Utilities;


namespace TheTechIdea.Beep.Container
{
    public static class RegisterBeep
    {
        public static IServiceCollection Services { get; private set; }

        private static IBeepService beepService;
        private static string BeepDataPath;
        private static bool mappingcreated = false;
        public static IBeepService Register(this IServiceCollection services, string directorypath, string containername, BeepConfigType configType, bool AddasSingleton = true)
        {
            Services = services;
            beepService = new   BeepService(services);
            beepService.Configure(directorypath, containername, configType, AddasSingleton);
            Services.AddSingleton<IBeepService>(beepService);
            BeepDataPath= ContainerMisc.CreateMainFolder();
            CreateMapping(beepService);
            return beepService;
        }
        public static IServiceCollection RegisterScoped(this IServiceCollection services)
        {
            Services = services;
            Services.AddScoped<IBeepService>();
            return Services;
        }
        public static IServiceCollection CreateMapping(this IBeepService beepService)
        {
            if(beepService!=null && !mappingcreated) {
                mappingcreated= true;
                ContainerMisc.AddAllConnectionConfigurations(beepService);
                ContainerMisc.AddAllDataSourceMappings(beepService);
                ContainerMisc.AddAllDataSourceQueryConfigurations(beepService);
            }
            return Services;
        }
        public static string GetMainFolder()
        {
            BeepDataPath= ContainerMisc.CreateMainFolder();
            return BeepDataPath;
        }
        public static IBeepService GetBeepService(this IDMEEditor dmeEditor)
        {

            return beepService;
        }

    }
}
