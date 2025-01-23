using Autofac;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Model;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.Tools;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TheTechIdea.Beep.Shared;

namespace TheTechIdea.Beep.Container.Services
{
    public class BeepService : IBeepService, IDisposable
    {
        private readonly IContainer _container;
        private bool disposedValue;
        private bool isassembliesloaded;
        private CancellationToken token;
        private bool isconfigloaded;

        #region "System Components"
        public IDMEEditor DMEEditor { get; set; }
        public IConfigEditor Config_editor { get; set; }
        public IDMLogger lg { get; set; }
        public IUtil util { get; set; }
        public IErrorsInfo Erinfo { get; set; }
        public IJsonLoader jsonLoader { get; set; }
        public IAssemblyHandler LLoader { get; set; }
        public IAppManager vis { get; set; }
        public string Containername { get; private set; }
        public BeepConfigType ConfigureationType { get; private set; }
        public string BeepDirectory { get; private set; }
        public Dictionary<EnvironmentType, IBeepEnvironment> Environments { get; set; }
        #endregion

        public BeepService(ContainerBuilder builder, string directorypath, string containername, BeepConfigType configType, bool AddasSingleton = false)
        {
            Containername = containername;
            ConfigureationType = configType;
            BeepDirectory = directorypath;

            // Register dependencies
            Configure(builder, directorypath, containername, configType, AddasSingleton);
            _container = builder.Build();
        }

        public void Configure(ContainerBuilder builder, string directorypath, string containername, BeepConfigType configType, bool AddasSingleton = false)
        {
            // Register all services with Autofac
            builder.RegisterType<DMLogger>().As<IDMLogger>().SingleInstance();
            builder.RegisterType<ConfigEditor>().As<IConfigEditor>().SingleInstance();
            builder.RegisterType<Util>().As<IUtil>().InstancePerDependency();
            builder.RegisterType<JsonLoader>().As<IJsonLoader>().InstancePerDependency();
            builder.RegisterType<AssemblyHandler>().As<IAssemblyHandler>().InstancePerDependency();
            builder.RegisterType<DMEEditor>().As<IDMEEditor>().SingleInstance();

            // If services need to be scoped
            if (!AddasSingleton)
            {
                LoadServicesScoped(builder);
            }
            else
            {
                LoadServicesSingleton(builder);
            }

            // Create default parameter object
            DMEEditor.Passedarguments = new PassedArgs();
            DMEEditor.Passedarguments.Objects = new List<ObjectItem>();
            DMEEditor.ErrorObject.Flag = Errors.Ok;

            // Load configurations if not loaded
            if (!isconfigloaded)
            {
                LoadConfigurations(containername);
                isconfigloaded = true;
            }
        }

        // Register services as Scoped
        public void LoadServicesScoped(ContainerBuilder builder)
        {
            builder.RegisterType<DMLogger>().As<IDMLogger>().InstancePerLifetimeScope();
            builder.RegisterType<ConfigEditor>().As<IConfigEditor>().InstancePerLifetimeScope();
            builder.RegisterType<DMEEditor>().As<IDMEEditor>().InstancePerLifetimeScope();
            builder.RegisterType<Util>().As<IUtil>().InstancePerLifetimeScope();
            builder.RegisterType<JsonLoader>().As<IJsonLoader>().InstancePerLifetimeScope();
            builder.RegisterType<AssemblyHandler>().As<IAssemblyHandler>().InstancePerLifetimeScope();
        }

        // Register services as Singleton
        public void LoadServicesSingleton(ContainerBuilder builder)
        {
            builder.RegisterInstance(lg).As<IDMLogger>();
            builder.RegisterInstance(Config_editor).As<IConfigEditor>();
            builder.RegisterInstance(DMEEditor).As<IDMEEditor>();
            builder.RegisterInstance(util).As<IUtil>();
            builder.RegisterInstance(jsonLoader).As<IJsonLoader>();
            builder.RegisterInstance(LLoader).As<IAssemblyHandler>();
        }

        public void LoadConfigurations(string containername)
        {
            if (isconfigloaded) return;
            isconfigloaded = true;
            ContainerMisc.AddAllConnectionConfigurations(this);
            ContainerMisc.AddAllDataSourceMappings(this);
            ContainerMisc.AddAllDataSourceQueryConfigurations(this);
            ContainerMisc.CreateMainFolder();
            ContainerMisc.CreateContainerfolder(containername);
        }

        public async Task LoadAssembliesAsync(Progress<PassedArgs> progress)
        {
            await Task.Run(() => LoadAssemblies(progress));
        }

        public void LoadAssemblies(Progress<PassedArgs> progress)
        {
            if (isassembliesloaded) return;
            isassembliesloaded = true;
            LLoader.LoadAllAssembly(progress, token);
            Config_editor.LoadedAssemblies = LLoader.Assemblies.Select(c => c.DllLib).ToList();
        }

        public void LoadEnvironments()
        {
            string envpath = Path.Combine(BeepDirectory, "Environments");
            if (!Directory.Exists(envpath)) return;

            Environments = new Dictionary<EnvironmentType, IBeepEnvironment>();
            string[] files = Directory.GetFiles(envpath, "*.json");
            foreach (string file in files)
            {
                string json = File.ReadAllText(file);
                IBeepEnvironment env = jsonLoader.DeserializeSingleObjectFromjsonString<IBeepEnvironment>(json);
                Environments.Add(env.EnvironmentType, env);
            }
        }

        public void SaveEnvironments()
        {
            string envpath = Path.Combine(BeepDirectory, "Environments");
            if (!Directory.Exists(envpath)) return;

            foreach (var env in Environments)
            {
                string json = jsonLoader.SerializeObject(env.Value);
                File.WriteAllText(Path.Combine(envpath, env.Value.EnvironmentName + ".json"), json);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;

            if (disposing)
            {
                DMEEditor?.Dispose();
                Config_editor?.Dispose();
                LLoader?.Dispose();
            }

            disposedValue = true;
        }
    }
}
