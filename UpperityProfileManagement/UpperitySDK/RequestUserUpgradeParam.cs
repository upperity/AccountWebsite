﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpperityProfileManagement.UpperitySDK
{
    public class RequestUserUpgradeParam
    {
        public string Login;
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
        public int RequestedUserLevel;
        public string Base64DocumentBinary;
        public string UserCertificatePIN;
        public string invoiceId;
    }
}