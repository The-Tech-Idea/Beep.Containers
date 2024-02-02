using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;
using TheTechIdea.Beep.Container.Models;

namespace Beep.Container.Model
{
    public class XrefUser
    {
        public int XrefUserID { get; set; }

        public string ParentUser { get; set; }
        public UserRelations Relation { get; set; }
    }
}
