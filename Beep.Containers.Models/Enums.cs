using System;
using System.Collections.Generic;
using System.Text;

namespace TheTechIdea.Beep.Container.Model
{
    public enum ServiceTypes
    {
        Database,File,API,Web,Service
    }
    public enum ServiceStatus
    {
        Active,Inactive,Disabled
    }
    public enum ServiceScope
    {
        Singleton,Scoped,Transient
    }
}
