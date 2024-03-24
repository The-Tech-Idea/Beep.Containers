using System.IO.Compression;
using Microsoft.Extensions.DependencyInjection;
using TheTechIdea.Beep.Container.Models;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Util;

namespace TheTechIdea.Beep.Container.ContainerManagement
{
    public  class CantainerManager : ICantainerManager,IDisposable
    {
        private IServiceCollection services;
        private bool disposedValue;

        public CantainerManager(IServiceCollection pservices)
        {
            services = pservices;
            Containers = new List<IBeepContainer>();
            ErrorsandMesseges = new ErrorsInfo();
            ContainerFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TheTechIdea", "Beep");
        }
        public List<IBeepContainer> Containers { get; set; }
        public ErrorsInfo ErrorsandMesseges { get; set; }

        public string ContainerFolderPath { get; set; }

        public async Task<ErrorsInfo> RemoveContainer(string pContainerName)
        {
            try
            {
                IErrorsInfo ErrorsandMesseges = new ErrorsInfo();
                // -- check if user already Exist
                var t = Task.Run<IBeepContainer>(() => Containers.Where(p => p.ContainerName.Equals(pContainerName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault());
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
        public async Task<ErrorsInfo> CreateContainer(string pContainerName, IServiceCollection pservices, string pContainerFolderPath=null)
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
                
                if (!Containers.Where(p => p.ContainerName.Equals(pContainerName, StringComparison.OrdinalIgnoreCase)).Any())
                {
                    IBeepContainer x = new BeepContainer() { ContainerName = pContainerName, ContainerFolderPath = pContainerFolderPath };
                    try
                    {
                        IBeepService beepservice = new BeepService(pservices, pContainerFolderPath, pContainerName, BeepConfigType.Container);
                        x.BeepService = beepservice;
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
        public async Task<ErrorsInfo> CreateContainer(string pContainerName, IServiceCollection pservices, string pContainerFolderPath, string pSecretKey, string pTokenKey)
        {
            try
            {
                IErrorsInfo ErrorsandMesseges = new ErrorsInfo();
                // -- check if Container already Exist

                ErrorsandMesseges= await CreateContainer(pContainerName, pservices, pContainerFolderPath);
                if(ErrorsandMesseges.Flag==Errors.Ok)
                {
                    IBeepContainer x = Containers.Where(p => p.ContainerName.Equals(pContainerName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
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
    }
}
