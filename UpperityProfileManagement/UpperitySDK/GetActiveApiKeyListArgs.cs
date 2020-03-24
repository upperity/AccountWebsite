using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpperityProfileManagement.UpperitySDK
{
    public class GetActiveApiKeyListArgs
    {
        public string action;
        public string ApiKey;
        public string SecretCode;
        public string CallBackURL;
        public PaiementCredentialEntity UserCredential = new PaiementCredentialEntity();
        public GetActiveApiKeyListParam param = new GetActiveApiKeyListParam();
    }
}