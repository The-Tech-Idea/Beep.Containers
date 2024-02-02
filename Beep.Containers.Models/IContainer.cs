using TheTechIdea.Beep.Container.Services;

namespace TheTechIdea.Beep.Container.Models
{
    public interface IContainer
    {
        IBeepService BeepService { get; set; }
        string AdminUserID { get; set; }
        string ContainerFolderPath { get; set; }
        int ContainerID { get; set; }
        string ContainerName { get; set; }
        string ContainerUrlPath { get; set; }
        List<string> Groups { get; set; }
        List<string> Products { get; set; }
        string GuidID { get; set; }
        List<string> Privileges { get; set; }
        List<string> Roles { get; set; }
        string SecretKey { get; set; }
        string TokenKey { get; set; }
        List<string> Users { get; set; }
         bool IsAdmin { get; set; }
         bool isActive { get;set; }
    }
}
