using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpperityProfileManagement.UpperitySDK
{
    public class GetCountryListArgs
    {
        public string action;
        public string ApiKey;
        public string SecretCode;
        public GetCountryListParam param = new GetCountryListParam();
    }
}