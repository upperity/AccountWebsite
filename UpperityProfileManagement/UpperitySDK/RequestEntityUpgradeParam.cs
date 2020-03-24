using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpperityProfileManagement.UpperitySDK
{
    public class RequestEntityUpgradeParam
    {
        public int ISOLanguageId;
	    public string EntityUpperityId;
	    public string CompanyName;
	    public List<AddressEntity> AdressList = new List<AddressEntity>();
	    public List<PhoneEntity> PhoneNumberList = new List<PhoneEntity>();
	    public DateTime DateOfIncorporation;
	    public string EntityType;
	    public string EntityJurisdiction;
	    public List<string> BoardMembersUserIdList = new List<string>();
	    public List<string> AuthorizedRepresentativesUserIdList = new List<string>();
	    public int RequestedEntityLevel;
	    public string Base64DocumentBinary;
	    public List<string> DirectorNameList = new List<string>();
	    public List<string> BeneficialOwnerNameList = new List<string>();
        public string invoiceId;
    }
}