using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpperityProfileManagement.UpperitySDK
{
    public class AddEntityParam
    {
        public int ISOLanguageId;
	    public string CompanyName;
	    public List<AddressEntity> AdressList = new List<AddressEntity>();
	    public List<PhoneEntity> PhoneNumberList = new List<PhoneEntity>();
	    public DateTime DateOfIncorporation;
	    public string EntityType;
	    public string EntityJurisdiction;
        public List<String> BoardMembersUserIdList = new List<String>();
	    public List<String> AuthorizedRepresentativesUserIdList = new List<String>();
	    public int CompanyLevel;
	    public List<String> AuthorizedProfileManagerUserIdList = new List<String>();
	    public List<String> VerificationContractAddrList = new List<String>();
	    public string EntityUserContractAddrList;
    }
}