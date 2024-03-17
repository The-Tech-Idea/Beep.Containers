using Beep.Vis.Module;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Logger;
using TheTechIdea.Tools;
using TheTechIdea.Util;

namespace TheTechIdea.Beep.Container.Services
{
    public class BeepService : IBeepService
    {
      
        public BeepService(IServiceCollection services, string directorypath,string containername, BeepConfigType configType)
        {
            Services = services;
            
            Configure(directorypath, containername, configType);
        }
        bool isDev = false;

        #region "System Components"
        public IDMEEditor DMEEditor { get; set; }
        public IConfigEditor Config_editor { get; set; }
        public IDMLogger lg { get; set; }
        public IUtil util { get; set; }
        public IErrorsInfo Erinfo { get; set; }
        public IJsonLoader jsonLoader { get; set; }
        public IAssemblyHandler LLoader { get; set; }
        public IServiceCollection Services { get; }
        public IVisManager vis { get; set; }
        public string  Containername { get; set; }
        CancellationTokenSource tokenSource;
        CancellationToken token;
        #endregion
        public void Configure(string directorypath , string containername, BeepConfigType configType) //ContainerBuilder builder
        {
            Erinfo = new ErrorsInfo();
            lg = new DMLogger();
            jsonLoader = new JsonLoader();
            string root = "";
            if(string.IsNullOrEmpty(directorypath) )
            {
                directorypath = AppContext.BaseDirectory;
            }
            root = Path.Combine(directorypath, "Beep");
            Config_editor = new ConfigEditor(lg, Erinfo, jsonLoader, root, containername, configType);
            util = new Util(lg, Erinfo, Config_editor);
            LLoader = new AssemblyHandler(Config_editor, Erinfo, lg, util);
            DMEEditor = new DMEEditor(lg, util, Erinfo, Config_editor, LLoader);
            try
            {
                Services.AddSingleton<IErrorsInfo>(Erinfo);
                Services.AddSingleton<IDMLogger>(lg);
                Services.AddSingleton<IConfigEditor>(Config_editor);
                Services.AddSingleton<IDMEEditor>(DMEEditor);
                Services.AddSingleton<IUtil>(util);
                Services.AddSingleton<IJsonLoader>(jsonLoader);
                // Create Default Parameter object
                DMEEditor.Passedarguments = new PassedArgs();
                DMEEditor.Passedarguments.Objects = new List<ObjectItem>();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
         
         
            // Setup the Entry Screen 
            // the screen has to be in one the Addin DLL's loaded by the Assembly loader


        }
        public void LoadAssemblies(Progress<PassedArgs> progress)
        {
            LLoader.LoadAllAssembly(progress, token);
            Config_editor.LoadedAssemblies = LLoader.Assemblies.Select(c => c.DllLib).ToList();
        }
        public void LoadAssemblies()
        {
            Progress<PassedArgs> progress=new Progress<PassedArgs>()
            {
            };
            LLoader.LoadAllAssembly(progress, token);
            Config_editor.LoadedAssemblies = LLoader.Assemblies.Select(c => c.DllLib).ToList();
        }

        
    
    }
}
