﻿using System.IO.Compression;
using Microsoft.Extensions.DependencyInjection;
using TheTechIdea.Beep.Container.Model;
using TheTechIdea.Beep.Container.Services;

using Newtonsoft.Json;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Utilities;
using Autofac;

namespace TheTechIdea.Beep.Container.ContainerManagement
{
    public  class CantainerManager : ICantainerManager,IDisposable
    {
        public CantainerManager()
        {
            

        }
        private IContainer services;
        public ContainerBuilder Builder { get; } // Replaced IServiceCollection with Autofac's ContainerBuilder

        private bool disposedValue;

        public IBeepContainer CurrentContainer { get; set; }
        public bool IsContainerActive { get; set; } = false;
        public bool IsContainerLoaded { get; set; } = false;
        public bool IsContainerCreated { get; set; } = false;

        public bool IsLogOn { get; set; } = false;
        public bool IsDataModified { get; set; } = false;
        public bool IsAssembliesLoaded { get; set; } = false;
        public bool IsBeepDataOn { get; set; } = false;
        public bool IsAppOn { get; set; } = false;
        public bool IsDevModeOn { get; set; } = false;
        public string filename { get; set; }="containers.json";
        public CantainerManager(ContainerBuilder builder)
        {
            Builder = builder;
         
            Containers = new List<IBeepContainer>();
            ErrorsandMesseges = new ErrorsInfo();
            CreateMainContainersfolder();
        }

        private List<IBeepContainer> _containers;
        public List<IBeepContainer> Containers
        {
            get
            {
                if (_containers == null)
                {
                    _containers = new List<IBeepContainer>();
                }
                return _containers;
            }
            set
            {
                _containers = value;
            }
        }

        public ErrorsInfo ErrorsandMesseges { get; set; } = new ErrorsInfo();
        string _containerFolderPath = string.Empty;
        public string ContainerFolderPath 
        { get { 
                if (string.IsNullOrEmpty(_containerFolderPath))
                {
                   CreateMainContainersfolder(); 
                 } 
                 return _containerFolderPath;
              }
          set 
              {
                _containerFolderPath = value;
              } 
        }
        private  void CreateMainContainersfolder()
        {
                if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TheTechIdea", "Beep")))
                {
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TheTechIdea", "Beep"));

                }
                string BeepDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TheTechIdea", "Beep");
            if (!Directory.Exists(Path.Combine(BeepDataPath, "Containers")))
            {
                Directory.CreateDirectory(Path.Combine(BeepDataPath, "Containers"));

            }
            _containerFolderPath = Path.Combine(BeepDataPath, "Containers");
            filename= Path.Combine(BeepDataPath, "Containers", "containers.json");
        }
        public  Task<ErrorsInfo> LoadContainers()
        {
            // load containers from file using system.text.json
            // load using System.Text.Json json file into Containers = new List<IBeepContainer>();
            try
            {
                if (File.Exists(filename))
                {
                    String JSONtxt = File.ReadAllText(filename);
                    Containers = JsonConvert.DeserializeObject<List<IBeepContainer>>(JSONtxt, GetSettings());
                }
                else
                {
                    Containers = new List<IBeepContainer>();
                }
            }
            catch (Exception ex)
            {
                ErrorsandMesseges.Flag = Errors.Failed;
                ErrorsandMesseges.Message = ex.Message;
                ErrorsandMesseges.Ex = ex;
                ErrorsandMesseges.Fucntion = "Load Containers ";
                ErrorsandMesseges.Module = "Container  Management";
            }
            
            return Task.FromResult(ErrorsandMesseges);
        }
        public Task<ErrorsInfo> SaveContainers()
        {
            // save containers to file using System.Text.Json
            try
            {
                using (StreamWriter file = File.CreateText(filename))
                {
                   
                        Newtonsoft.Json.JsonSerializer serializer = new JsonSerializer();
                        serializer.NullValueHandling = NullValueHandling.Include;
                        serializer.MissingMemberHandling = MissingMemberHandling.Ignore;
                        serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        serializer.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                }
            }
            catch (Exception ex)
            {
                ErrorsandMesseges.Flag = Errors.Failed;
                ErrorsandMesseges.Message = ex.Message;
                ErrorsandMesseges.Ex = ex;
                ErrorsandMesseges.Fucntion = "Load Containers ";
                ErrorsandMesseges.Module = "Container  Management";
            }
            return Task.FromResult(ErrorsandMesseges);
        }
        public List<IBeepContainer> GetUserContainers(string owner)
        {
            // get all conatiners for a user
            if(Containers==null)
            {
                Containers = new List<IBeepContainer>();
            }
            return Containers.Where(p => p.Owner.Equals(owner, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        public List<IBeepContainer> GetUserContainersByGuiID(string guidid)
        {
            // get all conatiners for a user
            if (Containers == null)
            {
                Containers = new List<IBeepContainer>();
            }
            return Containers.Where(p => p.OwnerGuidID.Equals(guidid, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        public List<IBeepContainer> GetUserContainers(int id)
        {
            if (Containers == null)
            {
                Containers = new List<IBeepContainer>();
            }
            // get all conatiners for a user
            return Containers.Where(p => p.OwnerID == id).ToList();
        }
        public IBeepContainer GetUserPrimaryContainer(string owner)
        {
            if (Containers == null)
            {
                Containers = new List<IBeepContainer>();
            }
            // get primary container for a user
            if (Containers.Where(p => p.Owner.Equals(owner, StringComparison.OrdinalIgnoreCase) && p.IsPrimary).Any())
            {
                CurrentContainer = Containers.Where(p => p.Owner.Equals(owner, StringComparison.OrdinalIgnoreCase) && p.IsPrimary).FirstOrDefault();
                IsContainerActive = true;
                IsContainerCreated = true;
                IsContainerLoaded = true;
                return CurrentContainer;
            }else
                if(Containers.Where(p => p.Owner.Equals(owner, StringComparison.OrdinalIgnoreCase)).Any())
            {
                IsContainerActive = true;
                IsContainerCreated = true;
                IsContainerLoaded = true;
                CurrentContainer = Containers.Where(p => p.Owner.Equals(owner, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                return CurrentContainer;
              
            }
            CurrentContainer= null;
            IsContainerActive = false;
            IsContainerCreated = false;
            IsContainerLoaded = false;

            return null;
           
        }
        public async Task<ErrorsInfo> RemoveContainer(string ContainerGuidID)
        {
            try
            {
                IErrorsInfo ErrorsandMesseges = new ErrorsInfo();
                // -- check if user already Exist
                var t = Task.Run<IBeepContainer>(() => Containers.Where(p => p.GuidID.Equals(ContainerGuidID, StringComparison.OrdinalIgnoreCase)).FirstOrDefault());
                t.Wait();

                if (t.Result == null)
                {
                    ErrorsandMesseges.Flag = Errors.Failed;
                    ErrorsandMesseges.Message = $"Container not Exists";
                }
                else
                {
                    t.Dispose();
                    Containers.Remove(t.Result);
                    ErrorsandMesseges.Flag = Errors.Ok;
                    ErrorsandMesseges.Message = $"Container Added";
                }

            }
            catch (Exception ex)
            {
                ErrorsandMesseges.Flag = Errors.Failed;
                ErrorsandMesseges.Message = ex.Message;
                ErrorsandMesseges.Ex = ex;
                ErrorsandMesseges.Fucntion = "Add Container";
                ErrorsandMesseges.Module = "Container  Management";

            }
            return await Task.FromResult(ErrorsandMesseges);
        }
        public async Task<ErrorsInfo> AddUpdateContainer(IBeepContainer pContainer)
        {
            try
            {
                IErrorsInfo ErrorsandMesseges = new ErrorsInfo();
                // -- check if Container already Exist

                if (!Containers.Where(p => p.ContainerName.Equals(pContainer.ContainerName, StringComparison.OrdinalIgnoreCase)).Any())
                {
                    Containers.Add(pContainer);
                    ErrorsandMesseges.Flag = Errors.Ok;
                    ErrorsandMesseges.Message = $"Container Added";
                }
                else
                {
                    int idx = Containers.FindIndex(p => p.ContainerName.Equals(pContainer.ContainerName, StringComparison.OrdinalIgnoreCase));
                    Containers[idx] = pContainer;
                    ErrorsandMesseges.Flag = Errors.Ok;
                    ErrorsandMesseges.Message = $"Container Updated";
                }

            }
            catch (Exception ex)
            {
                ErrorsandMesseges.Flag = Errors.Failed;
                ErrorsandMesseges.Message = ex.Message;
                ErrorsandMesseges.Ex = ex;
                ErrorsandMesseges.Fucntion = "Update/Add Container";
                ErrorsandMesseges.Module = "Container  Management";

            }
            return await Task.FromResult(ErrorsandMesseges);
        }
        public async Task<ErrorsInfo> CreateContainer(string ContainerGuidID,string owner, string ownerEmail, int ownerID, string ownerGuid, string pContainerName,  string pContainerFolderPath=null)
        {
            try
            {
                ErrorsInfo ErrorsandMesseges = new ErrorsInfo();
                // -- check if Container already Exist
                if(string.IsNullOrEmpty(pContainerName))
                {
                    ErrorsandMesseges.Flag = Errors.Failed;
                    ErrorsandMesseges.Message = $"Container Name is Empty";
                    return await Task.FromResult(ErrorsandMesseges);
                }
                if(string.IsNullOrEmpty(pContainerFolderPath))
                {
                    pContainerFolderPath = ContainerFolderPath;
                }
                IBeepContainer x = GetContainer( ContainerGuidID, owner, ownerEmail, ownerID, ownerGuid, pContainerName);
                if (x==null)
                {
                    x = new BeepContainer() { ContainerName = pContainerName, ContainerFolderPath = pContainerFolderPath };
                    try
                    {
                        IBeepService beepservice = new BeepService(Builder);
                        beepservice.Configure(pContainerFolderPath, pContainerName, BeepConfigType.Container);
                        x.BeepService = beepservice;
                        x.GuidID = Guid.NewGuid().ToString();
                        x.ContainerName = pContainerName;
                        x.AdminUserID = owner;
                        x.Owner = owner;
                        x.OwnerEmail = ownerEmail;
                        x.OwnerID = ownerID;
                        x.OwnerGuidID = ownerGuid;
                        x.ContainerFolderPath = pContainerFolderPath;
                        x.IsPrimary = true;
                        x.isActive = true;
                        x.GuidID = ContainerGuidID;
                        Containers.Add(x);
                        await CreateContainerFileSystem(x);
                        ErrorsandMesseges.Flag = Errors.Ok;
                        ErrorsandMesseges.Message = $"Container Added";
                    }
                    catch (Exception ex)
                    {
                        ErrorsandMesseges.Flag = Errors.Failed;
                        ErrorsandMesseges.Message = $"Container Failed : {ex.Message}";
                        
                    }
                 
                }
                else
                {
                    ErrorsandMesseges.Flag = Errors.Failed;
                    ErrorsandMesseges.Message = $"Container Exist";
                }

            }
            catch (Exception ex)
            {
                ErrorsandMesseges.Flag = Errors.Failed;
                ErrorsandMesseges.Message = ex.Message;
                ErrorsandMesseges.Ex = ex;
                ErrorsandMesseges.Fucntion = "Update/Add Container";
                ErrorsandMesseges.Module = "Container  Management";

            }
            return await Task.FromResult(ErrorsandMesseges);
        }
        private IBeepContainer GetContainer(string ContainerGuidID, string owner, string ownerEmail, int ownerID, string ownerGuid, string pContainerName)
        {
            // Get Container by all parameters owner, ownerEmail, ownerID, ownerGuid, pContainerName
            return Containers.Where(p => p.ContainerName.Equals(pContainerName, StringComparison.OrdinalIgnoreCase) && p.Owner.Equals(owner, StringComparison.OrdinalIgnoreCase) && p.OwnerEmail.Equals(ownerEmail, StringComparison.OrdinalIgnoreCase) && p.OwnerID == ownerID && p.OwnerGuidID.Equals(ownerGuid, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            
        }
        public async Task<ErrorsInfo> CreateContainer(string ContainerGuidID, string owner, string ownerEmail, int ownerID, string ownerGuid, string pContainerName, string pContainerFolderPath, string pSecretKey, string pTokenKey)
        {
            try
            {
                IErrorsInfo ErrorsandMesseges = new ErrorsInfo();
                // -- check if Container already Exist

                ErrorsandMesseges= await CreateContainer( ContainerGuidID, owner,  ownerEmail,  ownerID,  ownerGuid, pContainerName, pContainerFolderPath);
                if(ErrorsandMesseges.Flag==Errors.Ok)
                {
                    IBeepContainer x = GetContainer(ContainerGuidID, owner, ownerEmail, ownerID, ownerGuid, pContainerName); 
                    x.SecretKey = pSecretKey;
                    x.TokenKey = pTokenKey;
                   

                    ErrorsandMesseges = await AddUpdateContainer(x);
                }

            }
            catch (Exception ex)
            {
                ErrorsandMesseges.Flag = Errors.Failed;
                ErrorsandMesseges.Message = ex.Message;
                ErrorsandMesseges.Ex = ex;
                ErrorsandMesseges.Fucntion = "Update/Add Container";
                ErrorsandMesseges.Module = "Container  Management";

            }
            return await Task.FromResult(ErrorsandMesseges);
        }
        public async Task<ErrorsInfo> CreateContainerFileSystem(IBeepContainer pContainer)
        {
            try
            {
                IErrorsInfo ErrorsandMesseges = new ErrorsInfo();
                // -- check if Container already Exist

              //  ErrorsandMesseges = (IErrorsInfo)AddUpdateContainer(pContainer);
                //--------------------- Create File System -------------
                if (ErrorsandMesseges.Flag == Errors.Ok)
                {
                    if (pContainer.ContainerFolderPath != null)
                    {
                        try
                        {
                           string ExePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                         //   string ConatinerPath = Path.Combine(ExePath, pContainer.ContainerFolderPath);
                         if(File.Exists(Path.Combine(ExePath, "BeepContainerFiles.zip")))
                         {
                                await Task.Run(() => ZipFile.ExtractToDirectory(Path.Combine(ExePath, "BeepContainerFiles.zip"), Path.Combine(ContainerFolderPath,pContainer.ContainerName)));
                         }
                        }
                        #region "Catch for Zip file Extract"
                        catch (ArgumentNullException)
                        {
                            ErrorsandMesseges.Message = $"destinationDirectoryName or sourceArchiveFileName is null.";
                            ErrorsandMesseges.Flag = Errors.Failed;
                        }
                        catch (PathTooLongException)
                        {
                            ErrorsandMesseges.Flag = Errors.Failed;
                            ErrorsandMesseges.Message = $"The specified path in destinationDirectoryName or sourceArchiveFileName exceeds the system-defined maximum length.";
                        }
                        catch (DirectoryNotFoundException)
                        {
                            ErrorsandMesseges.Flag = Errors.Failed;
                            ErrorsandMesseges.Message = $"The specified path is invalid(for example, it is on an unmapped drive).";
                        }
                        catch (UnauthorizedAccessException)
                        {
                            ErrorsandMesseges.Message = $"The caller does not have the required permission to access the archive or the destination directory.";
                            ErrorsandMesseges.Flag = Errors.Failed;
                        }
                        catch (NotSupportedException)
                        {
                            ErrorsandMesseges.Message = $"destinationDirectoryName or sourceArchiveFileName contains an invalid format.";
                            ErrorsandMesseges.Flag = Errors.Failed;
                        }

                        catch (FileNotFoundException)
                        {
                            ErrorsandMesseges.Message = $"sourceArchiveFileName was not found.";
                            ErrorsandMesseges.Flag = Errors.Failed;
                        }
                        catch (InvalidDataException)
                        {
                            ErrorsandMesseges.Flag = Errors.Failed;
                            ErrorsandMesseges.Message = $"The archive specified by sourceArchiveFileName is not a valid zip archive.";
                            ErrorsandMesseges.Message += $"An archive entry was not found or was corrupt.";
                            ErrorsandMesseges.Message += $"An archive entry was compressed by using a compression method that is not supported.";
                        }
                        #endregion
                        //if (ErrorsandMesseges.Flag == Errors.Ok)
                        //{
                        //    //-------- Create DMService -----
                        //    try
                        //    {
                        //        services.AddScoped<IBeepService>(s => new BeepService(services, AppContext.BaseDirectory, pContainer.ContainerName, BeepConfigType.Container));
                        //        var provider = services.BuildServiceProvider();
                        //        var ContianerService = provider.GetService<IBeepService>();

                        //    }
                        //    catch (Exception dmex)
                        //    {
                        //        ErrorsandMesseges.Flag = Errors.Failed;
                        //        ErrorsandMesseges.Message = $"Failed to Create DMBeep Service - {dmex.Message}";
                        //    }
                        //}

                    }
                }

            }
            catch (Exception ex)
            {
                ErrorsandMesseges.Flag = Errors.Failed;
                ErrorsandMesseges.Message = ex.Message;
                ErrorsandMesseges.Ex = ex;
                ErrorsandMesseges.Fucntion = "Update/Add Container";
                ErrorsandMesseges.Module = "Container  Management";

            }
            return await Task.FromResult(ErrorsandMesseges);
        }
        public IBeepContainer GetBeepContainer(string ContainerName)
        {
            return Containers.Where(p => p.ContainerName.Equals(ContainerName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }
        public IBeepContainer GetBeepContainerByID(int ContainerID)
        {
            return Containers.Where(p => p.ContainerID == ContainerID).FirstOrDefault();
        }
        public IBeepContainer GetBeepContainerByGuidID(string ContainerGuidID)
        {
            return Containers.Where(p => p.GuidID == ContainerGuidID).FirstOrDefault();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~CantainerManager()
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
        private JsonSerializerSettings GetSettings()

        {
            return new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                //CheckAdditionalContent=true,
                //TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Converters = new List<JsonConverter> { new Newtonsoft.Json.Converters.StringEnumConverter() }


            };
        }
    }
}
