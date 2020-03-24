using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpperityProfileManagement.UpperitySDK
{
    public class AddEntityArgs
    {
        public string action;
        public string RequesterFondationUserId;
        public string RequesterFondationUserPassword;
        public string ApiKey;
        public string SecretCode;
        public string CallBackURL;
        public PaiementCredentialEntity UserCredential = new PaiementCredentialEntity();
        public AddEntityParam param = new AddEntityParam();
    }
}