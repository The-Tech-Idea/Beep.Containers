using Microsoft.Extensions.DependencyInjection;
using TheTechIdea.Beep.Container.Models;
using TheTechIdea.Util;

namespace TheTechIdea.Beep.Container.ContainerManagement
{
    public interface ICantainerManager
    {
        List<IContainer> Containers { get; set; }
        ErrorsInfo ErrorsandMesseges { get; set; }

        Task<ErrorsInfo> AddUpdateContainer(IContainer pContainer);
        Task<ErrorsInfo> CreateContainer(string pContainerName, IServiceCollection pservices, string pContainerFolderPath);
        Task<ErrorsInfo> CreateContainer(string pContainerName, IServiceCollection pservices, string pContainerFolderPath, string pSecretKey, string pTokenKey);
        Task<ErrorsInfo> CreateContainerFileSystem(IContainer pContainer);
        Task<ErrorsInfo> RemoveContainer(string pContainerName);
    }
}