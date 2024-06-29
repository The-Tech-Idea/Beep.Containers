 using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TheTechIdea.Beep.Container.Model;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Util;

namespace TheTechIdea.Beep.Container.ContainerManagement
{
    public static class RegisterContainerManagerServiceCollection
    {
        private static ICantainerManager cantainerManager;
        private static IServiceCollection Services;
       

        public static IServiceCollection AddBeepContainerManager(this IServiceCollection services)
        {
            Services = services;
            cantainerManager = new CantainerManager(services);
            services.AddSingleton<ICantainerManager>(cantainerManager);
            services.AddScoped<IBeepContainer, BeepContainer>();
            return services;
        }
        public static IServiceCollection AddBeepContainer(this IServiceCollection services, string directorypath, string containername, BeepConfigType configType, bool AddasSingleton = false)
        {
            Services = services;
            if (cantainerManager == null)
            {
                cantainerManager = new CantainerManager(services);
                services.AddSingleton<ICantainerManager>(cantainerManager);
            }

            BeepService beepService = new BeepService(services, directorypath, containername, configType, AddasSingleton);            services.AddSingleton<IBeepService>(beepService);
            services.AddSingleton<IBeepService>(beepService);
            BeepContainer container = new BeepContainer(containername, beepService);
            services.AddSingleton<IBeepContainer>(container);

            return services;
        }
        public static IServiceCollection AddBeepScopedContainerManager(this IServiceCollection services)
        {
            Services = services;
            if (cantainerManager == null)
            {
                cantainerManager = new CantainerManager(services);
                services.AddSingleton<ICantainerManager>(cantainerManager);
            }
            services.AddScoped<IBeepContainer,BeepContainer>();
            services.AddScoped<IBeepService,BeepService>();
            return services;
        }

    }
}
