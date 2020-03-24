using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpperityProfileManagement.UpperitySDK
{
    public class EntityInformation
    {
        public int ISOLanguageId;
	    public string CompanyName;
	    public List<AddressEntity> AdressList = new List<AddressEntity>();
	    public List<PhoneEntity> PhoneNumberList = new List<PhoneEntity>();
	    public DateTime DateOfIncorporation;
	    public string EntityType;
	    public string EntityJurisdiction;
	    public List<string> BoardMembersUserIdList = new List<string>();
	    public List<string> OfficerMembersUserIdList = new List<string>();
	    public List<string> AuthorizedRepresentativesUserIdList = new List<string>();
	    public List<string> BeneficialOwnersUserIdList = new List<string>();
        public int EntityTrustLevel;
	    public List<string> AuthorizedProfileManagerUserIdList = new List<string>();
	    public List<string> VerificationContractAddrList = new List<string>();
	    public string EntityUserContractAddrList;
	    public List<EntityUserRole> EntityUserList  = new List<EntityUserRole>();
	    public List<string> GovernanceContractAddrList = new List<string>();
        public List<EntityTypeEntity> entityTypeList = new List<EntityTypeEntity>();
        public List<UpgradeDocumentTypeEntity> UpgradeDocumentTypeList = new List<UpgradeDocumentTypeEntity>();
        public bool isInVerificationProcess;
        public bool IsConfirmed;
    }
}