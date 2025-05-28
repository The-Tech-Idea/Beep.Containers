using Autofac;
using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Container.Model;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Utilities;

namespace TheTechIdea.Beep.Container.ContainerManagement
{
    public static class RegisterContainer
    {
        private static ICantainerManager _containerManager;
        private static ContainerBuilder _builder;

        public static ContainerBuilder AddContainerManager(this ContainerBuilder builder)
        {
            _builder = builder;
            _containerManager = new CantainerManager();
            builder.RegisterInstance(_containerManager).As<ICantainerManager>().SingleInstance();
            builder.RegisterType<BeepContainer>().As<IBeepContainer>().InstancePerLifetimeScope();
            return builder;
        }

        public static ContainerBuilder AddContainer(this ContainerBuilder builder, string directorypath, string containername, BeepConfigType configType, bool addAsSingleton = false)
        {
            _builder = builder;
            if (_containerManager == null)
            {
                _containerManager = new CantainerManager();
                builder.RegisterInstance(_containerManager).As<ICantainerManager>().SingleInstance();
            }

            // Register BeepService
            var beepService = new BeepService();
            beepService.Configure(directorypath, containername, configType, addAsSingleton);
            builder.RegisterInstance(beepService).As<IBeepService>().SingleInstance();

            // Register BeepContainer
            var container = new BeepContainer(containername, beepService);
            builder.RegisterInstance(container).As<IBeepContainer>().SingleInstance();

            return builder;
        }

        public static ContainerBuilder AddScopedContainerManager(this ContainerBuilder builder)
        {
            _builder = builder;
            if (_containerManager == null)
            {
                _containerManager = new CantainerManager();
                builder.RegisterInstance(_containerManager).As<ICantainerManager>().SingleInstance();
            }

            // Register scoped services
            builder.RegisterType<BeepContainer>().As<IBeepContainer>().InstancePerLifetimeScope();
            builder.RegisterType<BeepService>().As<IBeepService>().InstancePerLifetimeScope();

            return builder;
        }
    }
}