using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpperityProfileManagement.UpperitySDK
{
    public class UserPersonalInformation
    {
        public int ISOLanguageId;
        public string UpperityId;
        public string firstName;
        public string middleName;
        public string LastName;
        public string gender;

        public List<AddressEntity> AddressList = new List<AddressEntity>();
        public List<EmailEntity> UserEmailList = new List<EmailEntity>();
        public List<PhoneEntity> phoneNumberList = new List<PhoneEntity>();

        public string Occupation;
        public DateTime BirthDate;
        public string BirthCity;
        public string BirthCountry;
        public string BirthMotherfullname;
        public string BirthFatherfullName;

        public List<string> VerificationContractAddrList = new List<string>();
        public List<AuthorizedUserEntity> AccessAuthorizationList = new List<AuthorizedUserEntity>();
        public int UserCertificationLevel;
        public bool isUserVerificationPending;
        public int verificationId;
        public int verificationStateId;
        public bool isConfirmed;
    }
}