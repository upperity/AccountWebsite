using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpperityProfileManagement.UpperitySDK
{
    public class GetActiveApiKeyListResponse
    {
        public bool Error;
        public int ErrorCode;
        public List<ApiKeyEntity> apiKeyList = new List<ApiKeyEntity>();
        public ApiKeyEntity serviceContractOwnerKey = new ApiKeyEntity();
        public bool isTcAccepted;
        public string currentEntity;

    }
}