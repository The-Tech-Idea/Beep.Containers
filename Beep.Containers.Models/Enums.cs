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
    public enum EnvironmentType
    {
        Development,Production,Test,Staging,QA,Training,Custom,NotSet,Live,LiveTest,LiveProduction
    }
}
