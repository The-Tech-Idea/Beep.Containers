using Beep.Vis.Module;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Logger;
using TheTechIdea.Tools;
using TheTechIdea.Util;

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
        Microsoft.Extensions.DependencyInjection.IServiceCollection Services { get; }
        IUtil util { get; set; }
        string  Containername { get; }
        BeepConfigType ConfigureationType { get; }
        string BeepDirectory { get; }
        void Configure(string directorypath, string containername, BeepConfigType configType);
        public IVisManager vis { get; set; }
        void LoadAssemblies(Progress<PassedArgs> progress);
        void LoadAssemblies();

    }
}