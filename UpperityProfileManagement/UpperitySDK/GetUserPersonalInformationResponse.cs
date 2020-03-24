using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace UpperityProfileManagement.UpperitySDK
{
    public class GetUserPersonalInformationResponse
    {
        public bool Error;
        public int ErrorCode;
        public UserPersonalInformation UserInformation = new UserPersonalInformation();
    }
}