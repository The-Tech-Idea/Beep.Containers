using Autofac;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Model;
using TheTechIdea.Beep.Container.Shared;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Logger;

using TheTechIdea.Beep.Tools;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Container.Services
{
    public class BeepService : IBeepService, IDisposable
    {
        public BeepService(ContainerBuilder builder)
        {
            Builder = builder;
            // Adding Required Configurations
        }

        public BeepService()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                ConfigureForDesignTime();
            }
        }

        private bool isDev = false;

        #region "System Components"
        public IDMEEditor DMEEditor { get; set; }
        public IConfigEditor Config_editor { get; set; }
        public IDMLogger lg { get; set; }
        public IUtil util { get; set; }
        public IErrorsInfo Erinfo { get; set; }
        public IJsonLoader jsonLoader { get; set; }
        public IAssemblyHandler LLoader { get; set; }
        public ContainerBuilder Builder { get; } // Replaced IServiceCollection with Autofac's ContainerBuilder
        public IAppManager vis { get; set; }
        public string Containername { get; private set; }
        public BeepConfigType ConfigureationType { get; private set; }
        public string BeepDirectory { get; private set; }

        private CancellationTokenSource tokenSource;
        private CancellationToken token;
        private bool disposedValue;
        private bool isconfigloaded = false;
        private bool isassembliesloaded = false;
        private bool isDesignTime;
        #endregion

        public void ConfigureForDesignTime()
        {
            try
            {
                Configure(AppContext.BaseDirectory, "DesignTimeContainer", BeepConfigType.DataConnector, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Design-time configuration failed: {ex.Message}");
            }
        }

        public void Configure(string directorypath, string containername, BeepConfigType configType, bool AddasSingleton = false)
        {
            Containername = containername;
            ConfigureationType = configType;
            BeepDirectory = directorypath;
            Erinfo = new ErrorsInfo();
            lg = new DMLogger();
            jsonLoader = new JsonLoader();
            string root = "";
            if (string.IsNullOrEmpty(directorypath))
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
                if (Builder != null)
                {
                    if (AddasSingleton == false)
                    {
                        LoadServicesScoped();
                    }
                    else
                    {
                        LoadServicesSingleton();
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

            if (isconfigloaded == false)
            {
                LoadConfigurations(containername);
                isconfigloaded = true;
            }
        }

        public void LoadServicesScoped()
        {
            // Register services as scoped using Autofac
            Builder.Register(c => lg).Keyed<IDMLogger>("Logger").InstancePerLifetimeScope();
            Builder.Register(c => Config_editor).Keyed<IConfigEditor>("ConfigEditor").InstancePerLifetimeScope();
            Builder.Register(c => DMEEditor).Keyed<IDMEEditor>("Editor").InstancePerLifetimeScope();
            Builder.Register(c => util).Keyed<IUtil>("Util").InstancePerLifetimeScope();
            Builder.Register(c => jsonLoader).Keyed<IJsonLoader>("JsonLoader").InstancePerLifetimeScope();
            Builder.Register(c => LLoader).Keyed<IAssemblyHandler>("AssemblyHandler").InstancePerLifetimeScope();
        }

        public void LoadServicesSingleton()
        {
            // Register services as singletons using Autofac
            Builder.RegisterInstance(lg).Keyed<IDMLogger>("Logger").SingleInstance();
            Builder.RegisterInstance(Config_editor).Keyed<IConfigEditor>("ConfigEditor").SingleInstance();
            Builder.RegisterInstance(DMEEditor).Keyed<IDMEEditor>("Editor").SingleInstance();
            Builder.RegisterInstance(util).Keyed<IUtil>("Util").SingleInstance();
            Builder.RegisterInstance(jsonLoader).Keyed<IJsonLoader>("JsonLoader").SingleInstance();
            Builder.RegisterInstance(LLoader).Keyed<IAssemblyHandler>("AssemblyHandler").SingleInstance();
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
            ContainerMisc.CreateMainFolder();
            ContainerMisc.CreateContainerfolder(containername);
        }

        public async Task LoadAssembliesAsync(Progress<PassedArgs> progress)
        {
            await Task.Run(() =>
            {
                LoadAssemblies(progress);
            });
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
            Progress<PassedArgs> progress = new Progress<PassedArgs>();
            LLoader.LoadAllAssembly(progress, token);
            Config_editor.LoadedAssemblies = LLoader.Assemblies.Select(c => c.DllLib).ToList();
        }

        public Dictionary<EnvironmentType, IBeepEnvironment> Environments { get; set; } = new Dictionary<EnvironmentType, IBeepEnvironment>();

        public void LoadEnvironments()
        {
            if (string.IsNullOrEmpty(ContainerMisc.ContainerDataPath))
            {
                ContainerMisc.CreateContainerfolder(Containername);
            }

            string envpath = Path.Combine(BeepDirectory, "Environments");
            if (Directory.Exists(envpath))
            {
                string[] files = Directory.GetFiles(envpath, "*.json");
                foreach (string file in files)
                {
                    string json = File.ReadAllText(file);
                    IBeepEnvironment env = jsonLoader.DeserializeSingleObjectFromjsonString<IBeepEnvironment>(json);
                    Environments.Add(env.EnvironmentType, env);
                }
            }
        }

        public void SaveEnvironments()
        {
            if (string.IsNullOrEmpty(ContainerMisc.ContainerDataPath))
            {
                ContainerMisc.CreateContainerfolder(Containername);
            }

            string envpath = Path.Combine(BeepDirectory, "Environments");
            if (Directory.Exists(envpath))
            {
                foreach (KeyValuePair<EnvironmentType, IBeepEnvironment> env in Environments)
                {
                    string json = jsonLoader.SerializeObject(env.Value);
                    File.WriteAllText(Path.Combine(envpath, env.Value.EnvironmentName + ".json"), json);
                }
            }
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DMEEditor?.Dispose();
                    Config_editor?.Dispose();
                    LLoader?.Dispose();
                }

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

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}