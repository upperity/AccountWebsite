using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpperityProfileManagement.UpperitySDK
{
    public class GetUserEntityListResponse
    {
        public bool Error;
        public int ErrorCode;
        public List<EntityUserRole> userRoleInformation = new List<EntityUserRole>();
    }
}