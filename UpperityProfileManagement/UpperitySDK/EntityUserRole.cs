using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpperityProfileManagement.UpperitySDK
{
    public class EntityUserRole
    {
        public string UpperityUserId;
        public string UpperityEntityId;
        public string EntityName;
        public string EntityType;
        public bool isEntityConfirmed;
        public string EntityJuridiction;
        public int EntityTrustLevel;
        public string UserFullName;
        public bool IsAdministrator;
        public List<string> RolesList = new List<string>();
    }
}