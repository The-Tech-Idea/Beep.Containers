using Microsoft.Extensions.DependencyInjection;
using TheTechIdea.Util;

namespace TheTechIdea.Beep.Container.Model
{
    public interface IBeepFeature
    {
        AssemblyClassDefinition AssemblyDefinition { get; set; }
        string AssemblyDescription { get; set; }
        string AssemblyName { get; set; }
        string AssemblyVersion { get; set; }
        string Description { get; set; }
        string FeatureID { get; set; }
        string GuidID { get; set; }
        bool IsConfigured { get; set; }
        bool IsEnabled { get; set; }
        bool IsLoaded { get; set; }
        bool IsScoped { get; set; }
        bool IsSingleton { get; set; }
        bool IsTransient { get; set; }
        string Name { get; set; }
        string Version { get; set; }

        IBeepContainer Container { get; set; }

        IErrorsInfo AddAsService(IServiceCollection services, ServiceScope scope = ServiceScope.Singleton);
        IErrorsInfo AddAsService(IServiceCollection services, string ServiceName, ServiceScope scope = ServiceScope.Singleton);
        IErrorsInfo AddAsService(IServiceCollection services, string ServiceName, int key, ServiceScope scope = ServiceScope.Singleton);
        IErrorsInfo Configure();
        IErrorsInfo Run();
    }
}