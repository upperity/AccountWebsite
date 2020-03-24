using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using UpperityProfileManagement.Models;
using System.Net.Http;
using System.Text;
using UpperityProfileManagement.UpperitySDK;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;


namespace UpperityProfileManagement.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        string UpperityAddr = ConfigurationManager.AppSettings["UpperityAPIUrl"];
        string ApiKey = ConfigurationManager.AppSettings["ApiKey"];
        string SecretCode = ConfigurationManager.AppSettings["SecretCode"];

        //GetCountryList
        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetCountryList()
        {
            HttpClient client = new HttpClient();
            GetCountryListArgs args = new GetCountryListArgs();

            args.action = "getCountryList";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            
            string json = new JavaScriptSerializer().Serialize(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            GetCountryListResponse response = JsonConvert.DeserializeObject<GetCountryListResponse>(responseContent.Result);

            return PartialView(response);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetStateList(String CountryId)
        {
            if (CountryId == null)
            {
                return PartialView(new GetStateListResponse());
            }
            else
            {
                HttpClient client = new HttpClient();
                GetStateListArgs args = new GetStateListArgs();

                args.action = "getStateList";
                args.ApiKey = ApiKey;
                args.SecretCode = SecretCode;
                args.param.CountryId = int.Parse(CountryId);
                string json = new JavaScriptSerializer().Serialize(args);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var x = client.PostAsync(UpperityAddr, content).Result;
                var responseContent = x.Content.ReadAsStringAsync();
                GetStateListResponse response = JsonConvert.DeserializeObject<GetStateListResponse>(responseContent.Result);

                return PartialView(response);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GenerateInvoice(int invoiceType, string invoiceLevel)
        {

            string invoiceIdRes = "";

            switch (invoiceLevel)
            {
                case "2":
                    if (invoiceType == 1)
                    {
                        invoiceIdRes = "31450596343946";
                    }
                    else
                    {
                        invoiceIdRes = "31450603520138";
                    }
                    break;
                case "3":
                    if (invoiceType == 1)
                    {
                        invoiceIdRes = "31450597785738";
                    }
                    else
                    {
                        invoiceIdRes = "31450644119690";
                    }
                    break;
                case "4":
                    if (invoiceType == 1)
                    {
                        invoiceIdRes = "31450635141258";
                    }
                    else
                    {
                        invoiceIdRes = "31450638090378";
                    }
                    break;
                case "5":
                    invoiceIdRes = "31450639958154";
                    break;
            }
            string producJson = "{\"draft_order\": {\"line_items\": [{\"variant_id\": " + invoiceIdRes + ",\"quantity\": 1      }    ]  }}";
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);

            var clientHandler = new HttpClientHandler
            {
                Credentials = new NetworkCredential("f7165733eee3d59cff0e1d2f5e85ab00", "c2f316dc2926303ad7015a0412f8a613"),
                PreAuthenticate = true,
                UseCookies = false
            };

            HttpClient client = new HttpClient(clientHandler);
                                   
            var content = new StringContent(producJson, Encoding.UTF8, "application/json");

            var x = client.PostAsync(new Uri("https://f7165733eee3d59cff0e1d2f5e85ab00:c2f316dc2926303ad7015a0412f8a613" + "@" + "upperitycom.myshopify.com/admin/api/2019-10/draft_orders.json"), content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<CDraftOrder>(responseContent.Result);

            invoiceData invoiceInfo = new invoiceData();
            invoiceInfo.invoiceId = response.DraftOrder.Id.ToString();
            invoiceInfo.invoiceURL = response.DraftOrder.InvoiceUrl.AbsoluteUri;

            return Json(invoiceInfo, JsonRequestBehavior.AllowGet);  
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult checkPaiementReceive(string invoiceId)
        {
            string producJson = "{\"draft_order\": {\"line_items\": [{\"variant_id\": 31450603520138,\"quantity\": 1      }    ]  }}";
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);

            var clientHandler = new HttpClientHandler
            {
                Credentials = new NetworkCredential("f7165733eee3d59cff0e1d2f5e85ab00", "c2f316dc2926303ad7015a0412f8a613"),
                PreAuthenticate = true,
                UseCookies = false
            };

            HttpClient client = new HttpClient(clientHandler);

           // var content = new StringContent(producJson, Encoding.UTF8, "application/json");

            var x = client.GetAsync(new Uri("https://f7165733eee3d59cff0e1d2f5e85ab00:c2f316dc2926303ad7015a0412f8a613" + "@" + "upperitycom.myshopify.com/admin/api/2020-01/draft_orders/" + invoiceId + ".json")).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<CDraftOrder>(responseContent.Result);


            InvoicePaiementInformation invoiceInfo = new InvoicePaiementInformation();
            invoiceInfo.invoiceId = invoiceId;
            invoiceInfo.isPaid = response.DraftOrder.Status == "completed" ? true : false;

            return Json(invoiceInfo, JsonRequestBehavior.AllowGet);  
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult UpdateUserProfile(object dump)
        {
            List<EmailEntity> emailList = new List<EmailEntity>();
            String[] spearator = { ","};
            //var x = Request.Form.;
            string[] allKey = Request.Form.AllKeys;
            List<string> emailKeyList = new List<string>();
            List<string> phoneKeyList = new List<string>();

            string phoneDefault = "";
            string emailDefault = "";

            foreach (string key in allKey)
            {
                if (key.Contains("email_"))
                {
                    emailKeyList.Add(key);
                }

                if (key.Contains("phone_"))
                {
                    phoneKeyList.Add(key);
                }

                if (key.Contains("emailDefault"))
                {
                    emailDefault = Request.Form[key];
                }

                if (key.Contains("phoneDefault"))
                {
                    phoneDefault = Request.Form[key];
                }
                
                    
            }

            
            UpdateUserPersonalInformationArgs args = new UpdateUserPersonalInformationArgs();
            args.action = "updateUserInformation";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.UserId = (string)Session["Login"];
            args.Password = (string)Session["Password"];
            args.UserCredential.UpperityUserId = (string)Session["Login"];
            args.UserCredential.UpperityPassword = (string)Session["Password"];

            AddressEntity currentAddress = new AddressEntity();

            if (emailKeyList.Any())
            {
                foreach(string key in emailKeyList)
                {
                    string emailIndex = key.Substring(key.IndexOf('_') + 1, key.Length - (key.IndexOf('_') + 1));
                    EmailEntity newEntry = new EmailEntity();

                    newEntry.email = Request.Form["email_" + emailIndex];
                    newEntry.emailType = Request.Form["emailType_" + emailIndex];
                    newEntry.isDefault = emailIndex == emailDefault;
                    args.param.UserEmailList.Add(newEntry);
                }
            }

            if (phoneKeyList.Any())
            {
                foreach (string key in phoneKeyList)
                {
                    string phoneIndex = key.Substring(key.IndexOf('_') + 1, key.Length - (key.IndexOf('_') + 1));
                    PhoneEntity newEntry = new PhoneEntity();

                    newEntry.phoneNumber = Request.Form["phone_" + phoneIndex];
                    newEntry.phoneType = Request.Form["phoneType_" + phoneIndex];
                    newEntry.isDefault = phoneIndex == phoneDefault;
                    args.param.phoneNumberList.Add(newEntry);
                }
            }

            int DefaultEmailCount = args.param.UserEmailList.Where(z => z.isDefault == true).Count();

            if (DefaultEmailCount == 0)
            {
                args.param.UserEmailList[0].isDefault = true;
            }

            int DefaultPhoneCount = args.param.phoneNumberList.Where(z => z.isDefault == true).Count();

            if (DefaultPhoneCount == 0)
            {
                args.param.phoneNumberList[0].isDefault = true;
            }

            currentAddress.BuildingName = Request.Form["buildingname"];
            currentAddress.BuildingNumber = Request.Form["buildingnumber"];
            currentAddress.City = Request.Form["city"];
            currentAddress.Country = Request.Form["usercountry"];
            currentAddress.ProvinceOrState = Request.Form["userState"];
            currentAddress.POBox = Request.Form["pobox"];
            currentAddress.PostalOrZipCode = Request.Form["postalcode"];
            
            currentAddress.StreetName = Request.Form["streetname"];
            currentAddress.SubUrb = Request.Form["suburb"];
            currentAddress.UnitNumber = Request.Form["unitnumber"];

      /*      args.param.BirthCity = Request.Form["email[]"];
            args.param.BirthCountry = Request.Form["email[]"];
            args.param.BirthDate = Request.Form["email[]"];
            args.param.BirthFatherfullName = Request.Form["email[]"];
            args.param.BirthMotherfullname = Request.Form["email[]"];*/
            args.param.firstName = Request.Form["firstname"];
      //      args.param.gender = Request.Form["email[]"];
            args.param.LastName = Request.Form["lastname"];
            args.param.UpperityId = (string)Session["Login"];
            args.param.middleName = Request.Form["middlename"];
      //      args.param.Occupation = Request.Form["email[]"];
            args.param.AddressList.Add(currentAddress);
            
            HttpClient client = new HttpClient();

            string json = JsonConvert.SerializeObject(args,Newtonsoft.Json.Formatting.Indented, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });
            string json2 = HttpUtility.JavaScriptStringEncode(json);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent =  x.Content.ReadAsStringAsync();

            UpdateUserPersonalInformationResponse response = JsonConvert.DeserializeObject<UpdateUserPersonalInformationResponse>(responseContent.Result);

            int y = 0;
            return RedirectToAction("UpperityUserProfile", "Account");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RenderUserSearch(string upcId)
        {
            HttpClient client = new HttpClient();
            GetUserPersonalInformationArgs args = new GetUserPersonalInformationArgs();

            args.action = "getUserInformations";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";
            args.RequesterUserId = (string)Session["Login"];// "TestRoleUser3";
            args.RequesterPassword = (string)Session["Password"];//"testUserPwd";
            args.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            args.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";
            args.param.UserId = upcId;

            string json = new JavaScriptSerializer().Serialize(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            GetUserPersonalInformationResponse response = JsonConvert.DeserializeObject<GetUserPersonalInformationResponse>(responseContent.Result);

            return PartialView(response);
            
        }

        [AllowAnonymous]
        public ActionResult RenderPartnerApiKeyList()
        {
            Session["EntityId"] = "";
            HttpClient client = new HttpClient();
            GetActiveApiKeyListArgs args = new GetActiveApiKeyListArgs();

            args.action = "getActiveApiKeyList";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";
            args.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            args.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";
            args.param.UpperityUserId = (string)Session["Login"];

            string json = new JavaScriptSerializer().Serialize(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            GetActiveApiKeyListResponse response = JsonConvert.DeserializeObject<GetActiveApiKeyListResponse>(responseContent.Result);

            GetUserPersonalInformationArgs argsUser = new GetUserPersonalInformationArgs();
            client = new HttpClient();
            argsUser.action = "getUserInformations";
            argsUser.ApiKey = ApiKey;
            argsUser.SecretCode = SecretCode;
            argsUser.CallBackURL = "assd";
            argsUser.RequesterUserId = (string)Session["Login"];// "TestRoleUser3";
            argsUser.RequesterPassword = (string)Session["Password"];//"testUserPwd";
            argsUser.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            argsUser.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";
            argsUser.param.UserId = (string)Session["Login"];

            string jsonUser = new JavaScriptSerializer().Serialize(argsUser);
            var contentUser = new StringContent(jsonUser, Encoding.UTF8, "application/json");

            var xUser = client.PostAsync(UpperityAddr, contentUser).Result;
            var responseContentUser = xUser.Content.ReadAsStringAsync();
            GetUserPersonalInformationResponse responseUser = JsonConvert.DeserializeObject<GetUserPersonalInformationResponse>(responseContentUser.Result);

            response.currentEntity = responseUser.UserInformation.firstName + " " + responseUser.UserInformation.LastName;
            return PartialView(response);

        }
        
            
        [AllowAnonymous]
        public ActionResult RenderBusinessPartnerApiKeyList()
        {
            HttpClient client = new HttpClient();
            GetEntityInformationArgs argsEntity = new GetEntityInformationArgs();

            argsEntity.action = "getEntityInformations";
            argsEntity.ApiKey = ApiKey;
            argsEntity.SecretCode = SecretCode;
            argsEntity.CallBackURL = "assd";
            argsEntity.RequesterUserId = (string)Session["Login"];// "TestRoleUser3";
            argsEntity.RequesterPassword = (string)Session["Password"];//"testUserPwd";
            argsEntity.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            argsEntity.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";
            argsEntity.param.EntityId = (string)Session["EntityId"];
            argsEntity.param.includeUserRoleList = true;

            string jsonEntity = new JavaScriptSerializer().Serialize(argsEntity);
            var contentEntity = new StringContent(jsonEntity, Encoding.UTF8, "application/json");

            var xEnt = client.PostAsync(UpperityAddr, contentEntity).Result;
            var responseEntityContent = xEnt.Content.ReadAsStringAsync();
            GetEntityInformationResponse responseEntity = JsonConvert.DeserializeObject<GetEntityInformationResponse>(responseEntityContent.Result);


            client = new HttpClient();
            GetActiveApiKeyListArgs args = new GetActiveApiKeyListArgs();

            args.action = "getActiveApiKeyList";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";
            args.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            args.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";
            args.param.UpperityUserId = (string)Session["Login"];
            args.param.UpperityEntityId = (string)Session["EntityId"];

            string json = new JavaScriptSerializer().Serialize(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            GetActiveApiKeyListResponse response = JsonConvert.DeserializeObject<GetActiveApiKeyListResponse>(responseContent.Result);
            response.currentEntity = responseEntity.entityInformation.CompanyName;
            return PartialView(response);
            
        }

        
        [AllowAnonymous]
        public ActionResult RenderBusinessPartnerServiceContractOwnerTC()
        {
            return PartialView();
        }

        [AllowAnonymous]
        public ActionResult RenderPartnerServiceContractOwnerTC()
        {
            return PartialView();
        }
        
        
        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddNewApiKey(string appName)
        {
            HttpClient client = new HttpClient();
            GenerateApiKeyArgs args = new GenerateApiKeyArgs();

            args.action = "generateApiKey";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";
            args.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            args.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";

            args.param.UpperityUserId = (string)Session["Login"];
            if ((string)Session["EntityId"] != "")
            {
                args.param.UpperityEntityId = (string)Session["EntityId"];
            }
            args.param.ApplicationName = appName;

            string json = new JavaScriptSerializer().Serialize(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            GenerateApiKeyResponse response = JsonConvert.DeserializeObject<GenerateApiKeyResponse>(responseContent.Result);

            return Content("");
            
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult removeApiKey(string apiKey, string secretCode)
        {
            HttpClient client = new HttpClient();
            RemoveApiKeyArgs args = new RemoveApiKeyArgs();

            args.action = "removeApiKey";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";
            args.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            args.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";

            args.param.ApiKey = apiKey;
            args.param.SecretCode = secretCode;
            

            string json = new JavaScriptSerializer().Serialize(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            RemoveApiKeyResponse response = JsonConvert.DeserializeObject<RemoveApiKeyResponse>(responseContent.Result);

            return Content("");
            
        }
        
        [HttpPost]
        [AllowAnonymous]
        public ActionResult RenderTCUserApiKey()
        {
            

            return PartialView();
            
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RenderTCEntityApiKey()
        {


            return PartialView();

        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult submitAccept()
        {

            HttpClient client = new HttpClient();
            addAcceptedApiKeyTCArgs args = new addAcceptedApiKeyTCArgs();

            args.action = "addAcceptedApiKeyTC";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";

            args.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            args.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";
            args.param.UpperityUserId = (string)Session["Login"];
            if ((string)Session["EntityId"] != "")
            {
                args.param.UpperityEntityId = (string)Session["EntityId"];
            }

            string json = new JavaScriptSerializer().Serialize(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            addAcceptedApiKeyTCResponse response = JsonConvert.DeserializeObject<addAcceptedApiKeyTCResponse>(responseContent.Result);

            return PartialView("~/Views/Account/addNewKeyAfterTC.cshtml");
            
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult submitEntityServiceContractOwnerAccept()
        {

            HttpClient client = new HttpClient();
            addAcceptedApiKeyTCArgs args = new addAcceptedApiKeyTCArgs();

            args.action = "addAcceptedApiKeyTC";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";

            args.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            args.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";
            args.param.UpperityUserId = (string)Session["Login"];
            if ((string)Session["EntityId"] != "")
            {
                args.param.UpperityEntityId = (string)Session["EntityId"];
            }

            args.param.isServiceContractOwner = true;

            string json = new JavaScriptSerializer().Serialize(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            addAcceptedApiKeyTCResponse response = JsonConvert.DeserializeObject<addAcceptedApiKeyTCResponse>(responseContent.Result);

            client = new HttpClient();
            GenerateServiceContractOwnerKeyArgs argsKey = new GenerateServiceContractOwnerKeyArgs();

            argsKey.action = "generateServiceContractOwnerKey";
            argsKey.ApiKey = ApiKey;
            argsKey.SecretCode = SecretCode;
            argsKey.CallBackURL = "assd";
            argsKey.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            argsKey.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";

            argsKey.param.UpperityUserId = (string)Session["Login"];

            if ((string)Session["EntityId"] != "")
            {
                argsKey.param.UpperityEntityId = (string)Session["EntityId"];
            }

            string jsonKey = new JavaScriptSerializer().Serialize(argsKey);
            var contentKey = new StringContent(jsonKey, Encoding.UTF8, "application/json");

            var xKey = client.PostAsync(UpperityAddr, contentKey).Result;
            var responseContentKey = xKey.Content.ReadAsStringAsync();
            GenerateServiceContractOwnerKeyResponse responseKey = JsonConvert.DeserializeObject<GenerateServiceContractOwnerKeyResponse>(responseContentKey.Result);

            return PartialView("~/Views/Account/addNewKeyAfterTC.cshtml");
            
        }

         [HttpPost]
        [AllowAnonymous]
        public ActionResult submitUserServiceContractOwnerAccept()
        {

            HttpClient client = new HttpClient();
            addAcceptedApiKeyTCArgs args = new addAcceptedApiKeyTCArgs();

            args.action = "addAcceptedApiKeyTC";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";

            args.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            args.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";
            args.param.UpperityUserId = (string)Session["Login"];
            if ((string)Session["EntityId"] != "")
            {
                args.param.UpperityEntityId = (string)Session["EntityId"];
            }

            args.param.isServiceContractOwner = true;

            string json = new JavaScriptSerializer().Serialize(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            addAcceptedApiKeyTCResponse response = JsonConvert.DeserializeObject<addAcceptedApiKeyTCResponse>(responseContent.Result);

            client = new HttpClient();
            GenerateServiceContractOwnerKeyArgs argsKey = new GenerateServiceContractOwnerKeyArgs();

            argsKey.action = "generateServiceContractOwnerKey";
            argsKey.ApiKey = ApiKey;
            argsKey.SecretCode = SecretCode;
            argsKey.CallBackURL = "assd";
            argsKey.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            argsKey.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";

            argsKey.param.UpperityUserId = (string)Session["Login"];

            if ((string)Session["EntityId"] != "")
            {
                argsKey.param.UpperityEntityId = (string)Session["EntityId"];
            }

            string jsonKey = new JavaScriptSerializer().Serialize(argsKey);
            var contentKey = new StringContent(jsonKey, Encoding.UTF8, "application/json");

            var xKey = client.PostAsync(UpperityAddr, contentKey).Result;
            var responseContentKey = xKey.Content.ReadAsStringAsync();
            GenerateServiceContractOwnerKeyResponse responseKey = JsonConvert.DeserializeObject<GenerateServiceContractOwnerKeyResponse>(responseContentKey.Result);

            return PartialView("~/Views/Account/addNewKeyAfterTC.cshtml");
            
        }
        
        
        [HttpPost]
        [AllowAnonymous]
        public ActionResult RenderEditUserRole(string userId)
        {
            HttpClient client = new HttpClient();
            GetEntityInformationArgs args = new GetEntityInformationArgs();

            args.action = "getEntityInformations";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";
            args.RequesterUserId = (string)Session["Login"];// "TestRoleUser3";
            args.RequesterPassword = (string)Session["Password"];//"testUserPwd";
            args.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            args.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";
            args.param.EntityId = (string)Session["EntityId"];
            args.param.includeUserRoleList = true;

            string json = new JavaScriptSerializer().Serialize(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            GetEntityInformationResponse response = JsonConvert.DeserializeObject<GetEntityInformationResponse>(responseContent.Result);

            return PartialView(response.entityInformation.EntityUserList.Where(y => y.UpperityUserId == userId).FirstOrDefault());
            
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EditUserRole(string userId, string roleList, bool isAdmin)
        {
            HttpClient client = new HttpClient();
            EditUserEntityRolesArgs args = new EditUserEntityRolesArgs();

            args.action = "editUserEntityRoles";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";
            args.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            args.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";
            args.param.EntityId = (string)Session["EntityId"];
            args.param.userInformation.UpperityUserId = userId;
            args.param.userInformation.IsAdministrator = isAdmin;
            args.param.userInformation.RolesList = roleList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();


            string json = new JavaScriptSerializer().Serialize(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            EditUserEntityRolesResponse response = JsonConvert.DeserializeObject<EditUserEntityRolesResponse>(responseContent.Result);

            return Content("");

        }
        
         [HttpPost]
        [AllowAnonymous]
        public ActionResult AddUserToEntity(string upcId)
        {
            HttpClient client = new HttpClient();
            AddUserToEntityArgs args = new AddUserToEntityArgs();

            args.action = "addEntityUser";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";
            args.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            args.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";
            args.param.EntityId = (string)Session["EntityId"];
            args.param.userInformation.UpperityUserId = upcId;

            string json = JsonConvert.SerializeObject(args, Newtonsoft.Json.Formatting.Indented, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            AddUserToEntityResponse response = JsonConvert.DeserializeObject<AddUserToEntityResponse>(responseContent.Result);

            return Content("");
            
        }

         [HttpPost]
        [AllowAnonymous]
         public ActionResult deleteUser(string userId)
        {
            HttpClient client = new HttpClient();
            RemoveUserFromEntityArgs args = new RemoveUserFromEntityArgs();

            args.action = "removeUserFromEntity";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";
            args.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            args.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";
            args.param.UpperityEntityId = (string)Session["EntityId"];
            args.param.UpperityUserId = userId;

            string json = JsonConvert.SerializeObject(args, Newtonsoft.Json.Formatting.Indented, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            RemoveUserFromEntityResponse response = JsonConvert.DeserializeObject<RemoveUserFromEntityResponse>(responseContent.Result);

            return Content("");
            
        }
        
        [HttpPost]
        [AllowAnonymous]
        public ActionResult CreateUser()
        {
            AddUserArgs args = new AddUserArgs();
            args.action = "addUser";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.UserCredential.UpperityUserId = (string)Session["Login"];
            args.UserCredential.UpperityPassword = (string)Session["Password"];

            AddressEntity currentAddress = new AddressEntity();

            EmailEntity newEntry = new EmailEntity();

            newEntry.email = Request.Form["email"];
            newEntry.emailType = "3";
            newEntry.isDefault = true;

            args.param.UserEmailList.Add(newEntry);
            args.param.Password = Request.Form["password"];
            args.param.Login = "temp";

            if ((Request.Form["phone"] != null) || (Request.Form["phone"] != ""))
            {
                PhoneEntity newEmailEntry = new PhoneEntity();

                newEmailEntry.phoneNumber = Request.Form["phone"];
                newEmailEntry.phoneType = "3";
                newEmailEntry.isDefault = true;
                args.param.phoneNumberList.Add(newEmailEntry);
            }

            currentAddress.BuildingName = Request.Form["buildingname"];
            currentAddress.BuildingNumber = Request.Form["buildingnumber"];
            currentAddress.City = Request.Form["city"];
            currentAddress.Country = Request.Form["usercountry"];
            currentAddress.ProvinceOrState = Request.Form["userState"];
            currentAddress.POBox = Request.Form["pobox"];
            currentAddress.PostalOrZipCode = Request.Form["postal"];

            currentAddress.StreetName = Request.Form["streetname"];
            currentAddress.SubUrb = Request.Form["suburb"];
            currentAddress.UnitNumber = Request.Form["unitnumber"];
            currentAddress.AddressType = "1";
            /*      args.param.BirthCity = Request.Form["email[]"];
                  args.param.BirthCountry = Request.Form["email[]"];
                  args.param.BirthDate = Request.Form["email[]"];
                  args.param.BirthFatherfullName = Request.Form["email[]"];
                  args.param.BirthMotherfullname = Request.Form["email[]"];*/
            args.param.firstName = Request.Form["firstname"];
            args.param.gender = Request.Form["gender"];
            args.param.LastName = Request.Form["lastname"];
            args.param.middleName = Request.Form["middlename"];
            //      args.param.Occupation = Request.Form["email[]"];
            args.param.AddressList.Add(currentAddress);
            args.param.ISOLanguageId = int.Parse(Request.Form["language"]);

            HttpClient client = new HttpClient();

            string json = JsonConvert.SerializeObject(args, Newtonsoft.Json.Formatting.Indented, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            AddUserResponse response = JsonConvert.DeserializeObject<AddUserResponse>(responseContent.Result);

            int y = 0;

            return Redirect("http://www.upperity.com/accountcreated/");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult UpdateEntityProfile(object dump)
        {
            List<EmailEntity> emailList = new List<EmailEntity>();
            
            UpdateEntityInformationArgs args = new UpdateEntityInformationArgs();

            args.action = "updateEntityInformation";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.RequesterUserId = (string)Session["Login"];
            args.RequesterPassword = (string)Session["Password"];
            args.UserCredential.UpperityUserId = (string)Session["Login"];
            args.UserCredential.UpperityPassword = (string)Session["Password"];
            args.EntityId = (string)Session["EntityId"];

            AddressEntity currentAddress = new AddressEntity();
            PhoneEntity newEntry = new PhoneEntity();

            newEntry.phoneNumber = Request.Form["phonenumber"];
            newEntry.phoneType = "2";

            args.param.PhoneNumberList.Add(newEntry);
            currentAddress.BuildingName = Request.Form["buildingname"];
            currentAddress.BuildingNumber = Request.Form["buildingnumber"];
            currentAddress.City = Request.Form["city"];
            currentAddress.Country = Request.Form["entitycountry"];
            currentAddress.ProvinceOrState = Request.Form["province"];
            currentAddress.POBox = Request.Form["pobox"];
            currentAddress.PostalOrZipCode = Request.Form["postalcode"];

            currentAddress.StreetName = Request.Form["streetname"];
            currentAddress.SubUrb = Request.Form["suburb"];
            currentAddress.UnitNumber = Request.Form["unitnumber"];
            currentAddress.AddressType = "2";

            args.param.CompanyName = Request.Form["entityname"];
            args.param.DateOfIncorporation = DateTime.Parse(Request.Form["incorporationDate"]);
            args.param.EntityJurisdiction = Request.Form["entityjuridiction"];
            args.param.EntityType = Request.Form["selectedEntityType"];
            args.param.ISOLanguageId = int.Parse(Request.Form["language"]);
            
            args.param.AdressList.Add(currentAddress);

            HttpClient client = new HttpClient();

            string json = JsonConvert.SerializeObject(args, Newtonsoft.Json.Formatting.Indented, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            UpdateEntityInformationResponse response = JsonConvert.DeserializeObject<UpdateEntityInformationResponse>(responseContent.Result);

            int y = 0;
            return RedirectToAction("UpperityEntityProfile", "Account");
        }

        
        [HttpPost]
        [AllowAnonymous]
        public ActionResult CreateNewEntityProfile()
        {
            List<EmailEntity> emailList = new List<EmailEntity>();
            HttpClient client = new HttpClient();
            AddEntityArgs args = new AddEntityArgs();

            args.action = "addEntity";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";
            args.RequesterFondationUserId = (string)Session["Login"];// "TestRoleUser3";
            args.RequesterFondationUserPassword = (string)Session["Password"];//"testUserPwd";
            args.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            args.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";
            args.param.CompanyName = Request.Form["entityName"];

            AddressEntity currentAddress = new AddressEntity();
            PhoneEntity newEntry = new PhoneEntity();

            newEntry.phoneNumber = Request.Form["phonenumber"];
            newEntry.phoneType = "2";
            newEntry.isDefault = true;

            args.param.PhoneNumberList.Add(newEntry);
            currentAddress.BuildingName = Request.Form["buildingname"];
            currentAddress.BuildingNumber = Request.Form["buildingnumber"];
            currentAddress.City = Request.Form["city"];
            currentAddress.Country = Request.Form["entitycountry"];
            currentAddress.ProvinceOrState = Request.Form["province"];
            currentAddress.POBox = Request.Form["pobox"];
            currentAddress.PostalOrZipCode = Request.Form["postalcode"];

            currentAddress.StreetName = Request.Form["streetname"];
            currentAddress.SubUrb = Request.Form["suburb"];
            currentAddress.UnitNumber = Request.Form["unitnumber"];
            currentAddress.AddressType = "2";

            args.param.CompanyName = Request.Form["entityname"];
            if ((Request.Form["incorporationDate"] != null) && (Request.Form["incorporationDate"] != ""))
            {
                args.param.DateOfIncorporation = DateTime.Parse(Request.Form["incorporationDate"]);
            }

            args.param.EntityJurisdiction = Request.Form["entityjuridiction"];
            args.param.EntityType = Request.Form["selectedEntityType"];
            args.param.ISOLanguageId = int.Parse(Request.Form["language"]);
            
            args.param.AdressList.Add(currentAddress);

            IsoDateTimeConverter formatSettings = new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy/MM/dd"
            };

            var json = JsonConvert.SerializeObject(args, formatSettings);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            AddEntityResponse response = JsonConvert.DeserializeObject<AddEntityResponse>(responseContent.Result);

            return PartialView("~/Views/Account/EntityCreatedSuccessfully.cshtml");
        }

        [AllowAnonymous]
        public ActionResult RenderCreateEntity()
        {
            Session["EntityId"] = "";

            HttpClient client = new HttpClient();
            GetEntityTypeListArgs argsEntity = new GetEntityTypeListArgs();

            argsEntity.action = "getEntityTypeList";
            argsEntity.ApiKey = ApiKey;
            argsEntity.SecretCode = SecretCode;
            argsEntity.CallBackURL = "assd";
            argsEntity.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            argsEntity.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";

            var json = new JavaScriptSerializer().Serialize(argsEntity);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var y = client.PostAsync(UpperityAddr, content).Result;
            var responseEntityTypeContent = y.Content.ReadAsStringAsync();
            GetEntityTypeListResponse responseEntityType = JsonConvert.DeserializeObject<GetEntityTypeListResponse>(responseEntityTypeContent.Result);

            return PartialView(responseEntityType);
        }

        [AllowAnonymous]
        public ActionResult EntityCreatedSuccessfully()
        {
            

            return PartialView();
        }
        
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ChangePassword()
        {
            HttpClient client = new HttpClient();
            ChangeUserPasswordArgs args = new ChangeUserPasswordArgs();

            args.action = "changeUserPassword";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.UserCredential.UpperityUserId = (string)Session["Login"];
            args.UserCredential.UpperityPassword = (string)Session["Password"];

            args.param.Login = (string)Session["Login"];
            args.param.OldPassword = Request.Form["currentpass"];
            args.param.NewPassword = Request.Form["password"];

            string json = new JavaScriptSerializer().Serialize(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            ChangeUserPasswordResponse response = JsonConvert.DeserializeObject<ChangeUserPasswordResponse>(responseContent.Result);

            if (response.Error)
            {

            }
            else
            {
                Session["Password"] = Request.Form["password"];
            }

            return RedirectToAction("UpperityUserProfile", "Account");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RenderAddUser()
        {
            return PartialView();
        }
        
        [AllowAnonymous]
        public ActionResult LogOut()
        {
            Session["Login"] = "";
            Session["Password"] = "";
            Session["EntityId"] = "";

            return Redirect("http://www.upperity.com/login");
        }
        
        [AllowAnonymous]
        public ActionResult AccountConfirmation(string id, string accountType, string userId)
        {
            HttpClient client = new HttpClient();
            ConfirmationProcessorArgs args = new ConfirmationProcessorArgs();

            args.action = "confirmationProcessor";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";

            if (accountType == null)
            {
                args.param.userId = id;
            }
            else
            {
                args.param.entityId = id;
                args.param.userId = userId;
            }

            string json = new JavaScriptSerializer().Serialize(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            ConfirmationProcessorResponse response = JsonConvert.DeserializeObject<ConfirmationProcessorResponse>(responseContent.Result);

            return PartialView();
        }

        [AllowAnonymous]
        public ActionResult ConfirmationCheck(string id, string accountType, string userId)
        {
            HttpClient client = new HttpClient();
            ConfirmationProcessorArgs args = new ConfirmationProcessorArgs();

            args.action = "confirmationCheck";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";

            if (accountType == null)
            {
                args.param.userId = id;
            }
            else
            {
                args.param.entityId = id;
                args.param.userId = userId;
            }

            string json = new JavaScriptSerializer().Serialize(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            ConfirmationProcessorResponse response = JsonConvert.DeserializeObject<ConfirmationProcessorResponse>(responseContent.Result);

            if (response.Error)
            {
                if (accountType == null)
                {
                    return Redirect("http://www.upperity.com/useraccountconfirmationError");
                }
                else
                {
                    return Redirect("http://www.upperity.com/entityconfirmationError/");

                }
                
            }
            else
            {
                if (accountType == null)
                {
                    return Redirect("http://www.upperity.com/useraccountconfirmation?id=" + id);
                }
                else
                {
                    return Redirect("http://www.upperity.com/entityaccountconfirmation/?id=" + id + "&accountType=1&userId=" + userId);

                }
            }
        }

        [AllowAnonymous]
        public ActionResult WebAccountConfirmation()
        {
            HttpClient client = new HttpClient();
            ConfirmationProcessorArgs args = new ConfirmationProcessorArgs();

            args.action = "confirmationProcessor";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";

            if (Request.Form["accountType"] == null)
            {
                args.param.userId = Request.Form["usernameconfirmed"];
            }
            else
            {
                args.param.entityId = Request.Form["entitynameconfirmed"];
                args.param.userId = Request.Form["userId"];
            }

            string json = new JavaScriptSerializer().Serialize(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            ConfirmationProcessorResponse response = JsonConvert.DeserializeObject<ConfirmationProcessorResponse>(responseContent.Result);

            return Redirect("http://www.upperity.com/login");
        }
        
        [AllowAnonymous]
        public ActionResult trustlevel()
        {
            HttpClient client = new HttpClient();
            GetUserPersonalInformationArgs args = new GetUserPersonalInformationArgs();

            args.action = "getUserInformations";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";
            args.RequesterUserId = (string)Session["Login"];// "TestRoleUser3";
            args.RequesterPassword = (string)Session["Password"];//"testUserPwd";
            args.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            args.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";
            args.param.UserId = (string)Session["Login"];

            string json = new JavaScriptSerializer().Serialize(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            GetUserPersonalInformationResponse response = JsonConvert.DeserializeObject<GetUserPersonalInformationResponse>(responseContent.Result);

            if (!response.UserInformation.AddressList.Any())
            {
                response.UserInformation.AddressList.Add(new AddressEntity());
            }

            int DefaultEmailCount = response.UserInformation.UserEmailList.Where(z => z.isDefault == true).Count();

            if (DefaultEmailCount == 0)
            {
                response.UserInformation.UserEmailList[0].isDefault = true;
            }

            int DefaultPhoneCount = response.UserInformation.phoneNumberList.Where(z => z.isDefault == true).Count();

            if (DefaultPhoneCount == 0)
            {
                response.UserInformation.phoneNumberList[0].isDefault = true;
            }

            return PartialView(response);
        }

        
        [AllowAnonymous]
        public ActionResult UpgradeUser()
        {

            List<EmailEntity> emailList = new List<EmailEntity>();
            String[] spearator = { "," };
            //var x = Request.Form.;
            string[] allKey = Request.Form.AllKeys;
            List<string> emailKeyList = new List<string>();
            List<string> phoneKeyList = new List<string>();

            string phoneDefault = "";
            string emailDefault = "";

            foreach (string key in allKey)
            {
                if (key.Contains("email_"))
                {
                    emailKeyList.Add(key);
                }

                if (key.Contains("phone_"))
                {
                    phoneKeyList.Add(key);
                }

                if (key.Contains("emailDefault"))
                {
                    emailDefault = Request.Form[key];
                }

                if (key.Contains("phoneDefault"))
                {
                    phoneDefault = Request.Form[key];
                }


            }

            string[] fileToRetreive = Request.Files.AllKeys;

            HttpClient client = new HttpClient();
            RequestUserUpgradeArgs args = new RequestUserUpgradeArgs();

            args.action = "requestUserUpgrade";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";
            args.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            args.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";

            AddressEntity currentAddress = new AddressEntity();

            if (emailKeyList.Any())
            {
                foreach (string key in emailKeyList)
                {
                    string emailIndex = key.Substring(key.IndexOf('_') + 1, key.Length - (key.IndexOf('_') + 1));
                    EmailEntity newEntry = new EmailEntity();

                    newEntry.email = Request.Form["email_" + emailIndex];
                    newEntry.emailType = emailTypeintToString(int.Parse(Request.Form["emailType_" + emailIndex]));
                    newEntry.isDefault = emailIndex == emailDefault;
                    args.param.UserEmailList.Add(newEntry);
                }
            }

            if (phoneKeyList.Any())
            {
                foreach (string key in phoneKeyList)
                {
                    string phoneIndex = key.Substring(key.IndexOf('_') + 1, key.Length - (key.IndexOf('_') + 1));
                    PhoneEntity newEntry = new PhoneEntity();

                    newEntry.phoneNumber = Request.Form["phone_" + phoneIndex];
                    newEntry.phoneType = Request.Form["phoneType_" + phoneIndex];
                    newEntry.isDefault = phoneIndex == phoneDefault;
                    args.param.phoneNumberList.Add(newEntry);
                }
            }

            int DefaultEmailCount = args.param.UserEmailList.Where(z => z.isDefault == true).Count();

            if (DefaultEmailCount == 0)
            {
                args.param.UserEmailList[0].isDefault = true;
            }

            int DefaultPhoneCount = args.param.phoneNumberList.Where(z => z.isDefault == true).Count();

            if (DefaultPhoneCount == 0)
            {
                args.param.phoneNumberList[0].isDefault = true;
            }

            currentAddress.BuildingName = Request.Form["buildingname"];
            currentAddress.BuildingNumber = Request.Form["buildingnumber"];
            currentAddress.City = Request.Form["city"];
            currentAddress.Country = Request.Form["usercountry"];
            currentAddress.ProvinceOrState = Request.Form["userState"];
            currentAddress.POBox = Request.Form["pobox"];
            currentAddress.PostalOrZipCode = Request.Form["postalcode"];

            currentAddress.StreetName = Request.Form["streetname"];
            currentAddress.SubUrb = Request.Form["suburb"];
            currentAddress.UnitNumber = Request.Form["unitnumber"];
            args.param.RequestedUserLevel = int.Parse(Request.Form["trustlevel"]);

            if (args.param.RequestedUserLevel > 2)
            {
                args.param.BirthCity = Request.Form["birthcity"];
                args.param.BirthCountry = Request.Form["birthcountry"];
                args.param.BirthDate = DateTime.Parse(Request.Form["dateofbirth"]);
                args.param.BirthFatherfullName = Request.Form["fathersname"];
                args.param.BirthMotherfullname = Request.Form["mothersname"];

                args.param.UserCertificatePIN = Request.Form["pin"];
            }
            args.param.invoiceId = Request.Form["finalinvoiceId"];
            args.param.firstName = Request.Form["firstname"];
            args.param.gender = Request.Form["GenderSelection"];
            args.param.LastName = Request.Form["lastname"];
            args.param.Login = (string)Session["Login"];
            args.param.middleName = Request.Form["middlename"];
            //      args.param.Occupation = Request.Form["email[]"];
            args.param.AddressList.Add(currentAddress);
            
            args.param.Base64DocumentBinary = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("temporaire"));
            
            string json = JsonConvert.SerializeObject(args, Newtonsoft.Json.Formatting.Indented, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            RequestUserUpgradeResponse response = JsonConvert.DeserializeObject<RequestUserUpgradeResponse>(responseContent.Result);


            if (!response.Error)
            {
                foreach (string fileEntry in fileToRetreive)
                {
                    var x2 = Request.Files[fileEntry];
                    if (x2.ContentLength != 0)
                    {
                        byte[] fileData = new byte[x2.ContentLength];
                        x2.InputStream.Read(fileData, 0, x2.ContentLength);

                        AddVerificationFileArgs addFileArgs = new AddVerificationFileArgs();

                        addFileArgs.action = "addVerificationFile";
                        addFileArgs.ApiKey = ApiKey;
                        addFileArgs.SecretCode = SecretCode;
                        addFileArgs.CallBackURL = "assd";
                        addFileArgs.UserCredential.UpperityUserId = (string)Session["Login"];
                        addFileArgs.UserCredential.UpperityPassword = (string)Session["Password"];
                        addFileArgs.param.Base64DocumentBinary = System.Convert.ToBase64String(fileData);
                        addFileArgs.param.DocumentName = Request.Form["filename" + fileEntry.Replace("file","")];
                        addFileArgs.param.DocumentType = 0;
                        addFileArgs.param.UpperityUserId = args.UserCredential.UpperityUserId;
                        json = JsonConvert.SerializeObject(addFileArgs, Newtonsoft.Json.Formatting.Indented, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });
                        content = new StringContent(json, Encoding.UTF8, "application/json");

                        var caller = client.PostAsync(UpperityAddr, content).Result;
                        responseContent = caller.Content.ReadAsStringAsync();
                        AddVerificationFileResponse responseFile = JsonConvert.DeserializeObject<AddVerificationFileResponse>(responseContent.Result);
                    }
                }
            }



            return RedirectToAction("UpperityUserProfile", "Account");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RequestUpgradeEntity()
        {
            List<EmailEntity> emailList = new List<EmailEntity>();
            string[] fileToRetreive = Request.Files.AllKeys;
            
            RequestEntityUpgradeArgs args = new RequestEntityUpgradeArgs();

            args.action = "requestEntityUpgrade";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.UserCredential.UpperityUserId = (string)Session["Login"];
            args.UserCredential.UpperityPassword = (string)Session["Password"];

            AddressEntity currentAddress = new AddressEntity();
            PhoneEntity newEntry = new PhoneEntity();

            newEntry.phoneNumber = Request.Form["phonenumber"];
            newEntry.phoneType = "2";
            args.param.EntityUpperityId = (string)Session["EntityId"];
            args.param.PhoneNumberList.Add(newEntry);
            currentAddress.BuildingName = Request.Form["buildingname"];
            currentAddress.BuildingNumber = Request.Form["buildingnumber"];
            currentAddress.City = Request.Form["city"];
            currentAddress.Country = Request.Form["entitycountry"];
            currentAddress.ProvinceOrState = Request.Form["province"];
            currentAddress.POBox = Request.Form["pobox"];
            currentAddress.PostalOrZipCode = Request.Form["postalcode"];
            currentAddress.AddressType = "2";
            currentAddress.StreetName = Request.Form["streetname"];
            currentAddress.SubUrb = Request.Form["suburb"];
            currentAddress.UnitNumber = Request.Form["unitnumber"];
            args.param.invoiceId = Request.Form["finalinvoiceId"];
            args.param.CompanyName = Request.Form["entityname"];
            args.param.DateOfIncorporation = DateTime.Parse(Request.Form["incorporationDate"]);
            args.param.EntityJurisdiction = Request.Form["entityjuridiction"];
            args.param.EntityType = Request.Form["selectedEntityType"];
            args.param.ISOLanguageId = int.Parse(Request.Form["language"]);
            args.param.RequestedEntityLevel = int.Parse(Request.Form["requestedTrustLevel"]);
            args.param.AdressList.Add(currentAddress);
            args.param.Base64DocumentBinary = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("temporaire"));
            HttpClient client = new HttpClient();

            string json = JsonConvert.SerializeObject(args, Newtonsoft.Json.Formatting.Indented, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            RequestEntityUpgradeResponse response = JsonConvert.DeserializeObject<RequestEntityUpgradeResponse>(responseContent.Result);

            if (!response.Error)
            {
                foreach (string fileEntry in fileToRetreive)
                {
                    var x2 = Request.Files[fileEntry];
                    if (x2.ContentLength != 0)
                    {
                        byte[] fileData = new byte[x2.ContentLength];
                        x2.InputStream.Read(fileData, 0, x2.ContentLength);

                        AddVerificationFileArgs addFileArgs = new AddVerificationFileArgs();

                        addFileArgs.action = "addVerificationFile";
                        addFileArgs.ApiKey = ApiKey;
                        addFileArgs.SecretCode = SecretCode;
                        addFileArgs.CallBackURL = "assd";
                        addFileArgs.UserCredential.UpperityUserId = (string)Session["Login"];
                        addFileArgs.UserCredential.UpperityPassword = (string)Session["Password"];
                        addFileArgs.param.Base64DocumentBinary = System.Convert.ToBase64String(fileData);
                        addFileArgs.param.DocumentName = Request.Form[fileEntry.Replace("file_", "")];
                        int last = fileEntry.LastIndexOf('_');
                        int total = fileEntry.Length;
                        addFileArgs.param.DocumentType = int.Parse(fileEntry.Substring(fileEntry.LastIndexOf('_') + 1, (fileEntry.Length - fileEntry.LastIndexOf('_') - 1)));
                        addFileArgs.param.UpperityUserId = args.UserCredential.UpperityUserId;
                        addFileArgs.param.UpperityEntityId = (string)Session["EntityId"];
                        json = JsonConvert.SerializeObject(addFileArgs, Newtonsoft.Json.Formatting.Indented, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });
                        content = new StringContent(json, Encoding.UTF8, "application/json");

                        var caller = client.PostAsync(UpperityAddr, content).Result;
                        responseContent = caller.Content.ReadAsStringAsync();
                        AddVerificationFileResponse responseFile = JsonConvert.DeserializeObject<AddVerificationFileResponse>(responseContent.Result);
                    }
                }
            }

            return RedirectToAction("UpperityEntityProfile", "Account");
        }
        [AllowAnonymous]
        public ActionResult trustlevelentity()
        {
            HttpClient client = new HttpClient();
            GetEntityInformationArgs args = new GetEntityInformationArgs();

            args.action = "getEntityInformations";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";
            args.RequesterUserId = (string)Session["Login"];// "TestRoleUser3";
            args.RequesterPassword = (string)Session["Password"];//"testUserPwd";
            args.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            args.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";
            args.param.EntityId = (string)Session["EntityId"];
            args.param.includeUserRoleList = true;

            string json = new JavaScriptSerializer().Serialize(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            GetEntityInformationResponse response = JsonConvert.DeserializeObject<GetEntityInformationResponse>(responseContent.Result);

            if (!response.entityInformation.AdressList.Any())
            {
                response.entityInformation.AdressList.Add(new AddressEntity());
            }

            client = new HttpClient();
            GetEntityTypeListArgs argsEntity = new GetEntityTypeListArgs();

            argsEntity.action = "getEntityTypeList";
            argsEntity.ApiKey = ApiKey;
            argsEntity.SecretCode = SecretCode;
            argsEntity.CallBackURL = "assd";
            argsEntity.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            argsEntity.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";

            json = new JavaScriptSerializer().Serialize(argsEntity);
            content = new StringContent(json, Encoding.UTF8, "application/json");

            var y = client.PostAsync(UpperityAddr, content).Result;
            var responseEntityTypeContent = y.Content.ReadAsStringAsync();
            GetEntityTypeListResponse responseEntityType = JsonConvert.DeserializeObject<GetEntityTypeListResponse>(responseEntityTypeContent.Result);
            response.entityInformation.entityTypeList = responseEntityType.entityTypeList;

            client = new HttpClient();
            GetUpgradeDocumentTypeListArgs argsDocEntity = new GetUpgradeDocumentTypeListArgs();

            argsDocEntity.action = "getUpgradeDocumentTypeList";
            argsDocEntity.ApiKey = ApiKey;
            argsDocEntity.SecretCode = SecretCode;
            argsDocEntity.CallBackURL = "assd";
            argsDocEntity.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            argsDocEntity.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";

            argsDocEntity.param.requestedEntityLevel = response.entityInformation.EntityTrustLevel + 1;
            argsDocEntity.param.entityTypeId = 1;
            response.entityInformation.EntityTrustLevel = response.entityInformation.EntityTrustLevel == 0 ? 1 : response.entityInformation.EntityTrustLevel;
            json = new JavaScriptSerializer().Serialize(argsDocEntity);
            content = new StringContent(json, Encoding.UTF8, "application/json");

            var v = client.PostAsync(UpperityAddr, content).Result;
            var responseEntityDocTypeContent = v.Content.ReadAsStringAsync();
            GetUpgradeDocumentTypeListResponse responseEntityDocType = JsonConvert.DeserializeObject<GetUpgradeDocumentTypeListResponse>(responseEntityDocTypeContent.Result);

            response.entityInformation.UpgradeDocumentTypeList = responseEntityDocType.UpgradeDocumentTypeList;
            AddressEntity currentAddress = new AddressEntity();
            PhoneEntity newEntry = new PhoneEntity();

            if (!response.entityInformation.AdressList.Any())
            {
                response.entityInformation.AdressList.Add(currentAddress);
            }

            if (!response.entityInformation.PhoneNumberList.Any())
            {
                response.entityInformation.PhoneNumberList.Add(newEntry);
            }
            return PartialView(response);
        }
        
        
        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetSelectedDocumentSet(DocumentSelectorEntity entityInformation)
        {
            HttpClient client = new HttpClient();
            GetUpgradeDocumentTypeListArgs argsDocEntity = new GetUpgradeDocumentTypeListArgs();

            argsDocEntity.action = "getUpgradeDocumentTypeList";
            argsDocEntity.ApiKey = ApiKey;
            argsDocEntity.SecretCode = SecretCode;
            argsDocEntity.CallBackURL = "assd";
            argsDocEntity.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            argsDocEntity.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";

            argsDocEntity.param.requestedEntityLevel = entityInformation.EntityTrustLevelId;
            argsDocEntity.param.entityTypeId = entityInformation.entityTypeId;

            var json = new JavaScriptSerializer().Serialize(argsDocEntity);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var v = client.PostAsync(UpperityAddr, content).Result;
            var responseEntityDocTypeContent = v.Content.ReadAsStringAsync();
            GetUpgradeDocumentTypeListResponse responseEntityDocType = JsonConvert.DeserializeObject<GetUpgradeDocumentTypeListResponse>(responseEntityDocTypeContent.Result);

            if (responseEntityDocType.UpgradeDocumentTypeList.Any())
            {
                return PartialView("~/Views/Account/RequiredDocumentList.cshtml", responseEntityDocType);
            }
            else
            {
                return Content("");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ValidateCurrentPassword(string currentPassword)
        {
            //a remplacer par le AuthenticateUser mes quil sois ajouter au SDK
            HttpClient client = new HttpClient();
            GetUserPersonalInformationArgs args = new GetUserPersonalInformationArgs();

            args.action = "getUserInformations";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";
            args.RequesterUserId = (string)Session["Login"];// "TestRoleUser3";
            args.RequesterPassword = currentPassword;//"testUserPwd";
            args.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            args.UserCredential.UpperityPassword = currentPassword; //"testUserPwd";
            args.param.UserId = (string)Session["Login"];

            string json = new JavaScriptSerializer().Serialize(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            GetUserPersonalInformationResponse response = JsonConvert.DeserializeObject<GetUserPersonalInformationResponse>(responseContent.Result);

            if (response.Error)
            {
                return Content("0");
            }
            else
            {
                return Content("1");
            }

        }

        [AllowAnonymous]
        public ActionResult UpperityUserPortal(string Login, string Password,string UpperityEntityId)
        {
            if (string.IsNullOrEmpty(UpperityEntityId))
            {
                Session["Login"] = Login.ToUpper();
                Session["Password"] = Password;
                return RedirectToAction("UpperityUserProfile", "Account");
            }
            else
            {
                Session["Login"] = Login.ToUpper();
                Session["Password"] = Password;
                Session["EntityId"] = UpperityEntityId;
                return RedirectToAction("UpperityEntityProfile", "Account");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult UpperityWebPortal()
        {
            Session["Login"] = Request.Form["username"].ToUpper();
            Session["Password"] = Request.Form["password"];

            return RedirectToAction("UpperityUserProfile", "Account");
        }
        
        //return RedirectToAction("Index", "Home");
        [AllowAnonymous]
        public ActionResult UpperityUserProfile()
        {
            Session["EntityId"] = "";

            HttpClient client = new HttpClient();

            GetUserPersonalInformationArgs args = new GetUserPersonalInformationArgs();

            args.action = "getUserInformations";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";
            args.RequesterUserId = (string)Session["Login"];// "TestRoleUser3";
            args.RequesterPassword = (string)Session["Password"];//"testUserPwd";
            args.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            args.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";
            args.param.UserId = (string)Session["Login"];
            //string json2 = JsonConvert.SerializeObject(args, Newtonsoft.Json.Formatting.Indented, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });
            string json = new JavaScriptSerializer().Serialize(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var x = client.PostAsync(UpperityAddr, content).Result;
            var byteArray = x.Content.ReadAsByteArrayAsync().Result;
            var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
            var responseContent =  x.Content.ReadAsStringAsync();
            string responsetxt = responseContent.Result;
          
            string conv4 = HttpUtility.HtmlDecode(responsetxt);

            Encoding iso = Encoding.GetEncoding("ISO-8859-1");
            Encoding utf8 = Encoding.UTF8;
            byte[] utfBytes = utf8.GetBytes(responsetxt);
            byte[] isoBytes = Encoding.Convert(utf8, iso, utfBytes);
            string msg = iso.GetString(isoBytes);
            GetUserPersonalInformationResponse response = JsonConvert.DeserializeObject<GetUserPersonalInformationResponse>(conv4);
            if (response.Error)
            {
                string oldUserName = (string)Session["Login"];
                Session["Login"] = "";
                Session["Password"] = "";
                Session["EntityId"] = "";

            return Redirect("http://www.upperity.com/loginfail?username=" + oldUserName);
                
            }

            if (!response.UserInformation.isConfirmed)
            {
                string oldUserName = (string)Session["Login"];
                Session["Login"] = "";
                Session["Password"] = "";
                Session["EntityId"] = "";
                return Redirect("http://www.upperity.com/loginfail?username=" + oldUserName + "&ltd=10");
            }
            if (!response.UserInformation.AddressList.Any())
            {
                response.UserInformation.AddressList.Add(new AddressEntity());
            }

            if (response.UserInformation.UserCertificationLevel == 0)
            {
                response.UserInformation.UserCertificationLevel = 1;
            }
            
            return PartialView(response);
        }

        [AllowAnonymous]
        public ActionResult RenderEntityProfile(string EntityID)
        {
            Session["EntityId"] = EntityID;

            return RedirectToAction("UpperityEntityProfile", "Account");
        }

        [AllowAnonymous]
        public ActionResult CreateEntity()
        {
            HttpClient client = new HttpClient();
            AddEntityArgs args = new AddEntityArgs();

            args.action = "addEntity";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";
            args.RequesterFondationUserId = (string)Session["Login"];// "TestRoleUser3";
            args.RequesterFondationUserPassword = (string)Session["Password"];//"testUserPwd";
            args.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            args.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";
            args.param.CompanyName = Request.Form["entityName"];

           // var ser = new JavaScriptSerializer();
           // ser.RegisterConverters(new[] { new DateTimeJavaScriptConverter() });
           // var jsonj = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
           // jsonj.SerializerSettings. = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat;
            IsoDateTimeConverter formatSettings = new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy/MM/dd"
            };
            //formatSettings.Formatting.
            var json = JsonConvert.SerializeObject(args, formatSettings);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            AddEntityResponse response = JsonConvert.DeserializeObject<AddEntityResponse>(responseContent.Result);

            //return PartialView("~/Views/Account/TC.cshtml", response.FondationEntityId);
            return RedirectToAction("ManageEntity", "Account");
        }
        
        [AllowAnonymous]
        public ActionResult UpperityEntityUserManagement()
        {
            HttpClient client = new HttpClient();
            GetEntityInformationArgs args = new GetEntityInformationArgs();

            args.action = "getEntityInformations";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";
            args.RequesterUserId = (string)Session["Login"];// "TestRoleUser3";
            args.RequesterPassword = (string)Session["Password"];//"testUserPwd";
            args.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            args.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";
            args.param.EntityId = (string)Session["EntityId"];
            args.param.includeUserRoleList = true;

            string json = new JavaScriptSerializer().Serialize(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            GetEntityInformationResponse response = JsonConvert.DeserializeObject<GetEntityInformationResponse>(responseContent.Result);

            if (!response.entityInformation.AdressList.Any())
            {
                response.entityInformation.AdressList.Add(new AddressEntity());
            }

            client = new HttpClient();
            GetEntityTypeListArgs argsEntity = new GetEntityTypeListArgs();

            argsEntity.action = "getEntityTypeList";
            argsEntity.ApiKey = ApiKey;
            argsEntity.SecretCode = SecretCode;
            argsEntity.CallBackURL = "assd";
            argsEntity.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            argsEntity.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";

            json = new JavaScriptSerializer().Serialize(argsEntity);
            content = new StringContent(json, Encoding.UTF8, "application/json");

            var y = client.PostAsync(UpperityAddr, content).Result;
            var responseEntityTypeContent = y.Content.ReadAsStringAsync();
            GetEntityTypeListResponse responseEntityType = JsonConvert.DeserializeObject<GetEntityTypeListResponse>(responseEntityTypeContent.Result);
            response.entityInformation.entityTypeList = responseEntityType.entityTypeList;

            foreach (var user in response.entityInformation.EntityUserList)
            {
                if (response.entityInformation.AuthorizedProfileManagerUserIdList.Contains(user.UpperityUserId))
                {
                    user.IsAdministrator = true;
                }
                //response.entityInformation.AuthorizedProfileManagerUserIdList
            }

            response.isCurrentUserAdministrator = response.entityInformation.AuthorizedProfileManagerUserIdList.Contains((string)Session["Login"]);

            return PartialView(response);
        }

        [AllowAnonymous]
        public ActionResult UpperityEntityProfile()
        {
            HttpClient client = new HttpClient();
            GetEntityInformationArgs args = new GetEntityInformationArgs();

            args.action = "getEntityInformations";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";
            args.RequesterUserId = (string)Session["Login"];// "TestRoleUser3";
            args.RequesterPassword = (string)Session["Password"];//"testUserPwd";
            args.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            args.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";
            args.param.EntityId = (string)Session["EntityId"];
            args.param.includeUserRoleList = true;

            string json = new JavaScriptSerializer().Serialize(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            GetEntityInformationResponse response = JsonConvert.DeserializeObject<GetEntityInformationResponse>(responseContent.Result);

            if (!response.entityInformation.AdressList.Any())
            {
                response.entityInformation.AdressList.Add(new AddressEntity());
            }

            client = new HttpClient();
            GetEntityTypeListArgs argsEntity = new GetEntityTypeListArgs();

            argsEntity.action = "getEntityTypeList";
            argsEntity.ApiKey = ApiKey;
            argsEntity.SecretCode = SecretCode;
            argsEntity.CallBackURL = "assd";
            argsEntity.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            argsEntity.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";

            json = new JavaScriptSerializer().Serialize(argsEntity);
            content = new StringContent(json, Encoding.UTF8, "application/json");

            var y = client.PostAsync(UpperityAddr, content).Result;
            var responseEntityTypeContent = y.Content.ReadAsStringAsync();
            GetEntityTypeListResponse responseEntityType = JsonConvert.DeserializeObject<GetEntityTypeListResponse>(responseEntityTypeContent.Result);
            response.entityInformation.entityTypeList = responseEntityType.entityTypeList;
            response.entityInformation.EntityTrustLevel = response.entityInformation.EntityTrustLevel != 0 ? response.entityInformation.EntityTrustLevel : 1;

            foreach (var user in response.entityInformation.EntityUserList)
            {
                if (response.entityInformation.AuthorizedProfileManagerUserIdList.Contains(user.UpperityUserId))
                {
                    user.IsAdministrator = true;
                }
                //response.entityInformation.AuthorizedProfileManagerUserIdList
            }

            response.isCurrentUserAdministrator = response.entityInformation.AuthorizedProfileManagerUserIdList.Contains((string)Session["Login"]);
            response.isCurrentUserAbleToUpdateProfile = true;

            if (response.entityInformation.EntityTrustLevel >= 2)
            {

                GetUserPersonalInformationArgs argsUser = new GetUserPersonalInformationArgs();

                argsUser.action = "getUserInformations";
                argsUser.ApiKey = ApiKey;
                argsUser.SecretCode = SecretCode;
                argsUser.CallBackURL = "assd";
                argsUser.RequesterUserId = (string)Session["Login"];// "TestRoleUser3";
                argsUser.RequesterPassword = (string)Session["Password"];//"testUserPwd";
                argsUser.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
                argsUser.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";
                argsUser.param.UserId = (string)Session["Login"];

                string jsonUser = new JavaScriptSerializer().Serialize(argsUser);
                var contentUser = new StringContent(jsonUser, Encoding.UTF8, "application/json");

                var userResponse = client.PostAsync(UpperityAddr, contentUser).Result;
                var responseUserContent = userResponse.Content.ReadAsStringAsync();
                GetUserPersonalInformationResponse responseUser = JsonConvert.DeserializeObject<GetUserPersonalInformationResponse>(responseUserContent.Result);
                if(responseUser.UserInformation.UserCertificationLevel < 3)
                {
                    response.isCurrentUserAbleToUpdateProfile = false;
                }
            }
            return PartialView(response);
        }

        [AllowAnonymous]
        public ActionResult ManageEntity()
        {
            Session["EntityId"] = "";

            HttpClient client = new HttpClient();
            GetUserEntityListArgs args = new GetUserEntityListArgs();

            args.action = "getUserEntityList";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";
            args.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            args.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";
            args.param.UpperityUserId = (string)Session["Login"];
            args.param.UpperityPassword = (string)Session["Password"];

            string json = new JavaScriptSerializer().Serialize(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            GetUserEntityListResponse response = JsonConvert.DeserializeObject<GetUserEntityListResponse>(responseContent.Result);

            return PartialView(response);
        }
        [AllowAnonymous]
        public ActionResult TcAcceptation()
        {
            HttpClient client = new HttpClient();
            GetUserEntityListArgs args = new GetUserEntityListArgs();

            args.action = "getUserEntityList";
            args.ApiKey = ApiKey;
            args.SecretCode = SecretCode;
            args.CallBackURL = "assd";
            args.UserCredential.UpperityUserId = (string)Session["Login"];//"TestRoleUser3";
            args.UserCredential.UpperityPassword = (string)Session["Password"]; //"testUserPwd";
            args.param.UpperityUserId = (string)Session["Login"];
            args.param.UpperityPassword = (string)Session["Password"];

            string json = new JavaScriptSerializer().Serialize(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var x = client.PostAsync(UpperityAddr, content).Result;
            var responseContent = x.Content.ReadAsStringAsync();
            GetUserEntityListResponse response = JsonConvert.DeserializeObject<GetUserEntityListResponse>(responseContent.Result);

            return RedirectToAction("ManageEntity", "Account");
        }
        

        //
        // GET: /Account/Login
        /*
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }*/

        private string emailTypeintToString(int emailTypeId)
        {
            string result = "";

            switch (emailTypeId)
            {
                case 1:
                    result = "Personal";
                    break;
                case 2:
                    result = "Enteprise";
                    break;
                case 3:
                    result = "Other";
                    break;
                default:
                    break;
            }

            return result;
        }

        private int emailTypeStringToInt(string emailStringType)
        {
            int result = 0;

            switch (emailStringType)
            {
                case "Personal":
                    result = 1;
                    break;
                case "Enteprise":
                    result = 2;
                    break;
                case "Other":
                    result = 3;
                    break;
                default:
                    break;
            }

            return result;
        }

        private string phoneTypeintToString(int emailTypeId)
        {
            string result = "";

            switch (emailTypeId)
            {
                case 1:
                    result = "Mobile";
                    break;
                case 2:
                    result = "Enteprise";
                    break;
                case 3:
                    result = "Other";
                    break;
                default:
                    break;
            }

            return result;
        }

        private int phoneTypeStringToInt(string emailStringType)
        {
            int result = 0;

            switch (emailStringType)
            {
                case "Mobile":
                    result = 1;
                    break;
                case "Enteprise":
                    result = 2;
                    break;
                case "Other":
                    result = 3;
                    break;
                default:
                    break;
            }

            return result;
        }

    }
}
