using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpperityProfileManagement.UpperitySDK
{
    public class GetEntityInformationResponse
    {
        public bool Error;
        public int ErrorCode;
        public EntityInformation entityInformation = new EntityInformation();
        public bool isCurrentUserAdministrator;
        public bool isCurrentUserAbleToUpdateProfile;
    }
}