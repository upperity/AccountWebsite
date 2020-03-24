using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpperityProfileManagement.UpperitySDK
{
    public class AddVerificationFileParam
    {
        public string UpperityUserId;
        public string UpperityEntityId;
        public int DocumentType;
        public string Base64DocumentBinary;
        public string DocumentName;
    }
}