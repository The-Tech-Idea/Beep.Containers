using Microsoft.Extensions.DependencyInjection;
using TheTechIdea.Beep.Container.Models;
using TheTechIdea.Util;

namespace TheTechIdea.Beep.Container.ContainerManagement
{
    public interface ICantainerManager
    {
        List<IBeepContainer> Containers { get; set; }
        ErrorsInfo ErrorsandMesseges { get; set; }

        Task<ErrorsInfo> AddUpdateContainer(IBeepContainer pContainer);
        Task<ErrorsInfo> CreateContainer(string pContainerName, IServiceCollection pservices, string pContainerFolderPath);
        Task<ErrorsInfo> CreateContainer(string pContainerName, IServiceCollection pservices, string pContainerFolderPath, string pSecretKey, string pTokenKey);
        Task<ErrorsInfo> CreateContainerFileSystem(IBeepContainer pContainer);
        Task<ErrorsInfo> RemoveContainer(string pContainerName);
    }
}