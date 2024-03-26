
using TheTechIdea.Beep.Container.Services;

namespace TheTechIdea.Beep.Container.Models
{
    public class BeepContainer:IBeepContainer

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
        public List<string> Modules { get; set; } = new List<string>();
        public List<string> Services { get; set; } = new List<string>();
        public List<string> Assemblies { get; set; } = new List<string>();
        public List<string> Configurations { get; set; } = new List<string>();
        public List<string> DataSources { get; set; } = new List<string>();
        public List<string> DataModels { get; set; } = new List<string>();
        public List<string> DataEntities { get; set; } = new List<string>();
        public List<string> DataViews { get; set; } = new List<string>();
        public List<string> DataTransformations { get; set; } = new List<string>();
        public List<string> DataExports { get; set; } = new List<string>();
        public List<string> DataImports { get; set; } = new List<string>();
        public List<string> DataConnections { get; set; } = new List<string>();
        public List<string> DataConnectionsConfigurations { get; set; } = new List<string>();
        public string Owner { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerGuidID { get; set; }
        public int OwnerID { get; set; }

        
        public int ContainerID { get; set; }
        public string ContainerFolderPath { get; set; }
        public string ContainerUrlPath { get; set; }
        public string SecretKey { get; set; }
        public string TokenKey { get; set; }
        public bool IsAdmin { get; set; }=false;
        public bool isActive { get; set; } = true;

    }
}
