using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpperityProfileManagement.UpperitySDK
{
    public class ConfirmationProcessorArgs
    {
        public String action;
        public string ApiKey;
        public string SecretCode;
        public String CallBackURL;
        public ConfirmationProcessorParam param = new ConfirmationProcessorParam();
    }
}