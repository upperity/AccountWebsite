using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpperityProfileManagement.UpperitySDK
{
    public class GetStateListArgs
    {
        public string action;
        public string ApiKey;
        public string SecretCode;
        public GetStateListParam param = new GetStateListParam();
    }
}