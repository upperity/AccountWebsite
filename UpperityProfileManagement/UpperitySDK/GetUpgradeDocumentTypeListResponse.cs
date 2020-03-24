using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpperityProfileManagement.UpperitySDK
{
    public class GetUpgradeDocumentTypeListResponse
    {
        public bool Error;
        public int ErrorCode;
        public List<UpgradeDocumentTypeEntity> UpgradeDocumentTypeList = new List<UpgradeDocumentTypeEntity>();
    }
}