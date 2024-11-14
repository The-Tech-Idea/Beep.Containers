using TheTechIdea.Beep.Vis.Modules;
using Microsoft.Extensions.DependencyInjection;

using TheTechIdea.Beep.Container.Model;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.Tools;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Container.Services
{
    public interface IBeepService
    {
      
        IConfigEditor Config_editor { get; set; }
        IDMEEditor DMEEditor { get; set; }
        IErrorsInfo Erinfo { get; set; }
        IJsonLoader jsonLoader { get; set; }
        IDMLogger lg { get; set; }
        IAssemblyHandler LLoader { get; set; }
        void LoadConfigurations(string containername);
        IServiceCollection Services { get; }
        void LoadServicesScoped(IServiceCollection services);
        void LoadServicesSingleton(IServiceCollection services);
        IUtil util { get; set; }
        string  Containername { get; }
        BeepConfigType ConfigureationType { get; }
        string BeepDirectory { get; }
        void Configure(string directorypath, string containername, BeepConfigType configType,bool AddasSingleton=false);
        public IVisManager vis { get; set; }
        void LoadAssemblies(Progress<PassedArgs> progress);
        Task LoadAssembliesAsync(Progress<PassedArgs> progress);
        void LoadAssemblies();

        Dictionary<EnvironmentType, IBeepEnvironment> Environments { get; set; }
        void LoadEnvironments();
        void SaveEnvironments();
        void Dispose();

    }
}