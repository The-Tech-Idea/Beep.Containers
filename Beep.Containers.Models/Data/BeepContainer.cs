
using TheTechIdea.Beep.Container.Services;

namespace TheTechIdea.Beep.Container.Models
{
    public class BeepContainer:IContainer

    {
        public BeepContainer()
        {

        }
        public BeepContainer(string containername)
        {
            GuidID = new Guid().ToString();

            ContainerName = containername;
        }

        public string ContainerName { get; set; }
        public IBeepService BeepService { get; set; }
        public string GuidID { get; set; }
        public List<string> Users { get; set; } = new List<string>();
        public string AdminUserID { get; set; } = string.Empty;
        public List<string> Groups { get; set; } = new List<string>();
        public List<string> Roles { get; set; } = new List<string>();
        public List<string> Privileges { get; set; } = new List<string>();
        public List<string> Products { get; set; }=new List<string>();
        
        public int ContainerID { get; set; }
        public string ContainerFolderPath { get; set; }
        public string ContainerUrlPath { get; set; }
        public string SecretKey { get; set; }
        public string TokenKey { get; set; }
        public bool IsAdmin { get; set; }=false;
        public bool isActive { get; set; } = true;

    }
}
