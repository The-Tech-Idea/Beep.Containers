using Beep.Vis.Module;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Logger;
using TheTechIdea.Tools;
using TheTechIdea.Util;

namespace TheTechIdea.Beep.Container.Services
{
    public class BeepService : IBeepService,IDisposable
    {

        public BeepService()
        {
            
        }
        public BeepService(IServiceCollection services, string directorypath,string containername, BeepConfigType configType)
        {
            Services = services;
            Containername=containername;
            ConfigureationType = configType;
            BeepDirectory= directorypath;
            Configure(directorypath, containername, configType);
            // Adding Required Configurations
         
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
        public string  Containername { get; private set; }
        public BeepConfigType ConfigureationType { get; private set; }
        public string BeepDirectory { get; private set; }

        CancellationTokenSource tokenSource;
        CancellationToken token;
        private bool disposedValue;
        private bool isconfigloaded = false;
        private bool isassembliesloaded=false;
        #endregion
        public void Configure(string directorypath , string containername, BeepConfigType configType, bool AddasSingleton = false) //ContainerBuilder builder
        {
            Containername = containername;
            ConfigureationType = configType;
            BeepDirectory = directorypath;
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
                if(Services!=null)
                {
                    if(AddasSingleton==false)
                    {
                        LoadServicesScoped(Services);
                    }
                    else
                    {
                        LoadServicesSingleton(Services);
                    }
                }
                
                // Create Default Parameter object
                DMEEditor.Passedarguments = new PassedArgs();
                DMEEditor.Passedarguments.Objects = new List<ObjectItem>();
                
                DMEEditor.ErrorObject.Flag = Errors.Ok;
            }
            catch (Exception ex)
            {
                DMEEditor.Passedarguments = new PassedArgs();
                DMEEditor.Passedarguments.Objects = new List<ObjectItem>();
                DMEEditor.ErrorObject.Ex = ex;
                DMEEditor.ErrorObject.Message = ex.Message;
                DMEEditor.ErrorObject.Flag = Errors.Failed;
                Console.WriteLine(ex.Message);
            }
            if(isconfigloaded==false)
            {
                LoadConfigurations(containername);
                isconfigloaded = true;
            }
            if(isassembliesloaded==false)
            {
                LoadAssemblies();
                isassembliesloaded = true;
            }
        }
        public void LoadServicesScoped(IServiceCollection services)
        {
            services.AddKeyedScoped<IDMLogger, DMLogger>("Logger");
            services.AddKeyedScoped<IConfigEditor, ConfigEditor>("ConfigEditor");
            services.AddKeyedScoped<IDMEEditor, DMEEditor>("Editor");
            services.AddKeyedScoped<IUtil, Util>("Util");
            services.AddKeyedScoped<IJsonLoader, JsonLoader>("JsonLoader");
            services.AddKeyedScoped<IAssemblyHandler, AssemblyHandler>("AssemblyHandler");
            
        }
        public void LoadServicesSingleton(IServiceCollection services)
        {
            services.AddKeyedSingleton<IDMLogger, DMLogger>("Logger");
            services.AddKeyedSingleton<IConfigEditor, ConfigEditor>("ConfigEditor");
            services.AddKeyedSingleton<IDMEEditor, DMEEditor>("Editor");
            services.AddKeyedSingleton<IUtil, Util>("Util");
            services.AddKeyedSingleton<IJsonLoader, JsonLoader>("JsonLoader");
            services.AddKeyedSingleton<IAssemblyHandler, AssemblyHandler>("AssemblyHandler");
        }
        public void LoadConfigurations(string containername)
        {
            if (isconfigloaded)
            {
                return;
            }
            isconfigloaded = true;
            ContainerMisc.AddAllConnectionConfigurations(this);
            ContainerMisc.AddAllDataSourceMappings(this);
            ContainerMisc.AddAllDataSourceQueryConfigurations(this);
            ContainerMisc.AddAllConnectionConfigurations(this);
            ContainerMisc.CreateMainFolder(this, containername);
        }
        public void LoadAssemblies(Progress<PassedArgs> progress)
        {
            if (isassembliesloaded)
            {
                return;
            }
            isassembliesloaded = true;
            LLoader.LoadAllAssembly(progress, token);
            Config_editor.LoadedAssemblies = LLoader.Assemblies.Select(c => c.DllLib).ToList();
        }
        public void LoadAssemblies()
        {
            if (isassembliesloaded)
            {
                return;
            }
            Progress<PassedArgs> progress=new Progress<PassedArgs>()
            {
            };
            LLoader.LoadAllAssembly(progress, token);
            Config_editor.LoadedAssemblies = LLoader.Assemblies.Select(c => c.DllLib).ToList();
        }
        public  virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects).
                    // Check each managed object to see if it implements IDisposable, then call Dispose on it.
                    DMEEditor?.Dispose();
                    Config_editor?.Dispose();
                    //lg?.Dispose();
                    //util?.Dispose();
                    //Erinfo?.Dispose();
                    //jsonLoader?.Dispose();
                    LLoader?.Dispose();

                    // If you're using any managed resources that need to be disposed, dispose them here.
                    // For example, if you have a Stream or a SqlConnection, dispose them here.
                    // stream?.Dispose();
                    // sqlConnection?.Dispose();
                }

                // Free unmanaged resources (unmanaged objects) and override the finalizer below.
                // Set large fields to null.
                DMEEditor = null;
                Config_editor = null;
                lg = null;
                util = null;
                Erinfo = null;
                jsonLoader = null;
                LLoader = null;

                disposedValue = true;
            }
        }
        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~BeepService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
