
using Beep.Vis.Module;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Workflow;
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
        Microsoft.Extensions.DependencyInjection.IServiceCollection Services { get; }
        IUtil util { get; set; }
        void Configure(string directorypath, string containername, BeepConfigType configType);
        public IVisManager vis { get; set; }
        void LoadAssemblies(Progress<PassedArgs> progress);
        void LoadAssemblies();

    }
}