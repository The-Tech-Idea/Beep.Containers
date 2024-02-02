using System.IO.Compression;
using Microsoft.Extensions.DependencyInjection;
using TheTechIdea.Beep.Container.Models;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Util;

namespace TheTechIdea.Beep.Container.ContainerManagement
{
    public class CantainerManager : ICantainerManager
    {
        private IServiceCollection services;
        public CantainerManager(IServiceCollection pservices)
        {
            services = pservices;
            Containers = new List<IContainer>();
            ErrorsandMesseges = new ErrorsInfo();
        }
        public List<IContainer> Containers { get; set; }
        public ErrorsInfo ErrorsandMesseges { get; set; }
       

        public async Task<ErrorsInfo> RemoveContainer(string pContainerName)
        {
            try
            {
                IErrorsInfo ErrorsandMesseges = new ErrorsInfo();
                // -- check if user already Exist
                var t = Task.Run<IContainer>(() => Containers.Where(p => p.ContainerName.Equals(pContainerName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault());
                t.Wait();

                if (t.Result == null)
                {
                    ErrorsandMesseges.Flag = Errors.Failed;
                    ErrorsandMesseges.Message = $"Container not Exists";
                }
                else
                {
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
        public async Task<ErrorsInfo> AddUpdateContainer(IContainer pContainer)
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
        public async Task<ErrorsInfo> CreateContainer(string pContainerName, IServiceCollection pservices, string pContainerFolderPath)
        {
            try
            {
                IErrorsInfo ErrorsandMesseges = new ErrorsInfo();
                // -- check if Container already Exist

                if (!Containers.Where(p => p.ContainerName.Equals(pContainerName, StringComparison.OrdinalIgnoreCase)).Any())
                {
                    BeepContainer x = new BeepContainer() { ContainerName = pContainerName, ContainerFolderPath = pContainerFolderPath };
                    BeepService beepservcie = new BeepService(pservices,AppContext.BaseDirectory, pContainerName, BeepConfigType.Container);

                    Containers.Add(x);
                    ErrorsandMesseges.Flag = Errors.Ok;
                    ErrorsandMesseges.Message = $"Container Added";
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

                if (!Containers.Where(p => p.ContainerName.Equals(pContainerName, StringComparison.OrdinalIgnoreCase)).Any())
                {
                    BeepContainer x = new BeepContainer() { ContainerName = pContainerName, ContainerFolderPath = pContainerFolderPath, SecretKey = pSecretKey, TokenKey = pTokenKey };
                    Containers.Add(x);
                    ErrorsandMesseges.Flag = Errors.Ok;
                    ErrorsandMesseges.Message = $"Container Added";
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
        public async Task<ErrorsInfo> CreateContainerFileSystem(IContainer pContainer)
        {
            try
            {
                IErrorsInfo ErrorsandMesseges = new ErrorsInfo();
                // -- check if Container already Exist

                ErrorsandMesseges = (IErrorsInfo)AddUpdateContainer(pContainer);
                //--------------------- Create File System -------------
                if (ErrorsandMesseges.Flag == Errors.Ok)
                {
                    if (pContainer.ContainerFolderPath != null)
                    {
                        try
                        {
                            string ExePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                            string ConatinerPath = Path.Combine(ExePath, pContainer.ContainerFolderPath);
                            await Task.Run(() => ZipFile.ExtractToDirectory(Path.Combine(ExePath, "BeepContainerFiles.zip"), ConatinerPath));

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
                        if (ErrorsandMesseges.Flag == Errors.Ok)
                        {
                            //-------- Create DMService -----
                            try
                            {
                                services.AddScoped<IBeepService>(s => new BeepService(services, AppContext.BaseDirectory, pContainer.ContainerName, BeepConfigType.Container));
                                var provider = services.BuildServiceProvider();
                                var ContianerService = provider.GetService<IBeepService>();

                            }
                            catch (Exception dmex)
                            {
                                ErrorsandMesseges.Flag = Errors.Failed;
                                ErrorsandMesseges.Message = $"Failed to Create DMBeep Service - {dmex.Message}";
                            }
                        }

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

    
    }
}
