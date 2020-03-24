using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpperityProfileManagement.UpperitySDK
{
    public class UpdateEntityInformationArgs
    {
        public string action;
        public string RequesterUserId;
        public string RequesterPassword;
        public string ApiKey;
        public string SecretCode;
        public string CallBackURL;
        public string EntityId;
        public PaiementCredentialEntity UserCredential = new PaiementCredentialEntity();
        public EntityInformation param = new EntityInformation();
    }
}