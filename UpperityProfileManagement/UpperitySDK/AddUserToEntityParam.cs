using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpperityProfileManagement.UpperitySDK
{
    public class AddUserToEntityParam
    {
        public string EntityId;
        public EntityUserRole userInformation = new EntityUserRole();
    }
}