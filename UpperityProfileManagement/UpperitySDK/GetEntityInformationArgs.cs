using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpperityProfileManagement.UpperitySDK
{
    public class GetEntityInformationArgs
    {
        public string action;
        public string RequesterUserId;
        public string RequesterPassword;
        public string ApiKey;
        public string SecretCode;
        public string CallBackURL;
        public PaiementCredentialEntity UserCredential = new PaiementCredentialEntity();
        public GetEntityInformationParam param = new GetEntityInformationParam();
    }
}