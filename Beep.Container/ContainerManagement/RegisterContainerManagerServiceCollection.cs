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
        public static IServiceCollection AddBeepScopedContainerManager(this IServiceCollection services)
        {
            Services = services;
            //cantainerManager = new CantainerManager(services);
            services.AddScoped<ICantainerManager,CantainerManager>();
            services.AddScoped<IBeepContainer,BeepContainer>();
            services.AddScoped<IBeepService,BeepService>();
            return services;
        }

    }
}
