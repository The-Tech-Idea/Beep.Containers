using Autofac;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Container.Shared;
using TheTechIdea.Beep.Editor;

using TheTechIdea.Beep.Utilities;

namespace TheTechIdea.Beep.Container
{
    public static class RegisterBeepinAutofac
    {
        private static ContainerBuilder _builder;
        private static IContainer _autofacContainer;

        private static IBeepService _beepService;
        private static string _beepDataPath;
        private static bool _mappingCreated = false;

        public static ContainerBuilder RegisterBeep(this ContainerBuilder builder, string directorypath, string containername, BeepConfigType configType, bool addAsSingleton = true)
        {
            _builder = builder;

            // Create and configure BeepService
            _beepService = new BeepServiceAutoFac(builder);
            _beepService.Configure(directorypath, containername, configType, addAsSingleton);

            // Register BeepService with Autofac
            if (addAsSingleton)
            {
                builder.RegisterInstance(_beepService).As<IBeepService>().SingleInstance();
            }
            else
            {
                builder.RegisterInstance(_beepService).As<IBeepService>().InstancePerLifetimeScope();
            }

            // Create the main folder and mappings
            _beepDataPath = ContainerMisc.CreateMainFolder();
            CreateBeepMapping(builder);

            return builder;
        }

        public static ContainerBuilder RegisterScopedBeep(this ContainerBuilder builder)
        {
            _builder = builder;

            // Register BeepService as a scoped service
            builder.RegisterType<BeepServiceAutoFac>().As<IBeepService>().InstancePerLifetimeScope();

            return builder;
        }

        public static ContainerBuilder CreateBeepMapping(this ContainerBuilder builder)
        {
            if (_beepService != null && !_mappingCreated)
            {
                _mappingCreated = true;
                ContainerMisc.AddAllConnectionConfigurations(_beepService);
                ContainerMisc.AddAllDataSourceMappings(_beepService);
                ContainerMisc.AddAllDataSourceQueryConfigurations(_beepService);
            }

            return builder;
        }

        public static string GetMainFolder()
        {
            _beepDataPath = ContainerMisc.CreateMainFolder();
            return _beepDataPath;
        }

        public static IBeepService GetBeepService(this IDMEEditor dmeEditor)
        {
            return _beepService;
        }

        public static IContainer BuildContainer()
        {
            if (_builder != null)
            {
                _autofacContainer = _builder.Build();
            }
            return _autofacContainer;
        }
    }
}