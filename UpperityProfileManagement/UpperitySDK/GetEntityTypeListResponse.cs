using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpperityProfileManagement.UpperitySDK
{
    public class GetEntityTypeListResponse
    {
        public bool Error;
        public int ErrorCode;
        public List<EntityTypeEntity> entityTypeList = new List<EntityTypeEntity>();
    }
}