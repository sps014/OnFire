using OnFire.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnFire.Auth
{
    public class EMailAuth
    {

        RestClient client = new RestClient();
        public string AuthKey { get; set; }
        public EMailAuth(string AuthKey)
        {
            this.AuthKey = AuthKey;
        }
        public async Task<User> SignUpUser(string email,string password)
        {
            UserPayload p = new UserPayload() { email = email, password = password, returnSecureToken = true };
            string URL = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=" + AuthKey;
            var data = await RecieveData(URL, p);
            return CreateUser(data, password);

        }
        public async Task<User> SignInUser(string email, string password)
        {
            UserPayload p = new UserPayload() { email = email, password = password, returnSecureToken = true };
            string URL = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=" + AuthKey;
            var data=await RecieveData(URL, p);
            return CreateSignInUser(data, password);
        }

        private User CreateUser(Dictionary<string,object> res, string password)
        {
            User user = new User();
            user.Email = res["email"].ToString();
            user.Password = password;
            user.UID = res["localId"].ToString();
            user.IdToken = res["idToken"].ToString();
            user.RefreshToken = res["refreshToken"].ToString();
            user.Kind = res["kind"].ToString();
            user.ExpiresIn = res["expiresIn"].ToString();

            return user;
        }

        private User CreateSignInUser(Dictionary<string, object> res, string password)
        {
            User user = new User();
            user.Email = res["email"].ToString();
            user.Password = password;
            user.UID = res["localId"].ToString();
            user.IdToken = res["idToken"].ToString();
            user.RefreshToken = res["refreshToken"].ToString();
            user.Kind = res["kind"].ToString();
            user.ExpiresIn = res["expiresIn"].ToString();
            user.DisplayName = res["displayName"].ToString();
            user.Regisered = bool.Parse(res["registered"].ToString());
            return user;
        }
        public async void SendResetPassword(string email)
        {
            ResetPasswordPayload p = new ResetPasswordPayload() { email = email };
            string URL = "https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key=" + AuthKey;

            await RecieveData(URL, p);

        }
        public async void SendVerification(string idToken)
        {
            VerificationPayload p = new VerificationPayload() { idToken = idToken,requestType= "VERIFY_EMAIL" };
            string URL = "https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key=" + AuthKey;
            await RecieveData(URL, p);

        }
        public async Task<UserInfo> GetUser(string idToken)
        {
            IdPayload p = new IdPayload() { idToken = idToken};
            string URL = "https://identitytoolkit.googleapis.com/v1/accounts:lookup?key=" + AuthKey;
            var data = await RecieveData(URL, p);
            return CreateUserInfo(data);


        }
        public async Task<Dictionary<string, object>> RecieveData<T>(string URL, T payload)
        {
            var res = await client.Post(URL, payload);


            if (res.IsSuccessStatusCode)
            {
                Stream m = await res.Content.ReadAsStreamAsync();
                Dictionary<string, object> data = await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(m);
                return data;
            }
            else
            {
                string error = await res.Content.ReadAsStringAsync();
                int i = error.IndexOf("message") + 7 + 4;
                int last = error.IndexOf("\n", i) - 2;
                string subError = error.Substring(i, last - i);
                subError = subError.Replace("\\\"", "");
                throw new Exception(subError);
            }

        }

        private UserInfo CreateUserInfo(Dictionary<string,object> datap)
        {
            UserInfo info = new UserInfo();
            var data = JsonSerializer.Serialize(datap);
            info.CreatedAt = extractField(data, "createdAt");
            info.CustomAuth = extractField(data, "customAuth");
            //info.Disabled = bool.Parse(extractField(data, "disabled"));
            info.DisplayName = extractField(data, "displayName");
            info.Email = extractField(data, "email");
            info.EmailVerified = bool.Parse(extractField(data, "emailVerified"));
            info.LastLoginAt = extractField(data, "lastLoginAt");
            info.LocalId = extractField(data, "localId");
            info.PasswordHash = extractField(data, "passwordHash");
            info.PasswordUpdatedAt = double.Parse(extractField(data, "passwordUpdatedAt"));
            info.PhotoUrl = extractField(data, "photoUrl");
            info.ValidSince = extractField(data, "validSince");


            //info.CreatedAt=data["createdAt"].ToString();
            //info.CustomAuth= data["customAuth"].ToString();
            //info.Disabled = bool.Parse(data["disabled"].ToString());
            //info.DisplayName = data["displayName"].ToString();
            //info.Email = data["email"].ToString();
            //info.EmailVerified =bool.Parse(data["emailVerified"].ToString());
            //info.LastLoginAt = data["lastLoginAt"].ToString();
            //info.LocalId = data["localId"].ToString();
            //info.PasswordHash = data["passwordHash"].ToString();
            //info.PasswordUpdatedAt = double.Parse(data["passwordUpdatedAt"].ToString());
            //info.PhotoUrl = data["photoUrl"].ToString();
            //info.ProviderUserInfo = data["providerUserInfo"] as object[];
            //info.ValidSince = data["validSince"].ToString();

            return info;


        }
        private string extractField(string data,string key)
        {
            if (data.IndexOf(key) < 0)
                return null;
            int i = data.IndexOf(key)+key.Length + 2;
            int last = data.IndexOf(",", i);
            string subData = data.Substring(i, last - i);
            subData = subData.Replace("\\\"", "");
            subData = subData.Replace("\"", "");

            return subData;
        }
        public async void ChangeEmail(string email, string idToken)
        {
            EmailPayload p = new EmailPayload() { email = email, idToken = idToken };
            string URL = "https://identitytoolkit.googleapis.com/v1/accounts:update?key=" + AuthKey;
            var res = await client.Post(URL, p);
            if (res.IsSuccessStatusCode)
            {
                Stream m = await res.Content.ReadAsStreamAsync();
                Dictionary<string, object> data = await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(m);
            }
            else
            {
                //todo
            }

        }
        public async void ChangePassword(string password,string idToken)
        {
            PasswordPayload p = new PasswordPayload() { password=password,idToken=idToken };
            string URL = "https://identitytoolkit.googleapis.com/v1/accounts:update?key=" + AuthKey;
            var res = await client.Post(URL, p);
            if (res.IsSuccessStatusCode)
            {
                Stream m = await res.Content.ReadAsStreamAsync();
                Dictionary<string, object> data = await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(m);
            }
            else
            {
                //todo
            }

        }
        

        //Update profile info todo


        class UserPayload
        {
            public string email { get; set; }
            public string password { get; set; }
            public bool returnSecureToken { get; set; }
        }
        class IdPayload
        {
            public string idToken { get; set; }
        }
        class ResetPasswordPayload
        {
            public string requestType { get; set; } = "PASSWORD_RESET";
            public string email { get; set; }
        }
        class VerificationPayload
        {
            public string requestType { get; set; } = "PASSWORD_RESET";
            public string idToken { get; set; }
        }
        class EmailPayload
        {
            public string idToken { get; set; }
            public string email { get; set; }
            public bool returnSecureToken { get; set; } = true;
        }
        class PasswordPayload
        {
            public string idToken { get; set; }
            public string password { get; set; }
            public bool returnSecureToken { get; set; } = true;
        }

        public class User
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string UID { get; set; }
            public string IdToken { get; set; }
            public string RefreshToken { get; set; }
            public string Kind { get; set; }
            public string ExpiresIn { get; set; }
            public string DisplayName { get; set; } = "";
            public bool Regisered { get; set; } = false;
        }
        public class UserInfo
        {
            public string Email { get; set; }
            public string LocalId { get; set; }
            public string DisplayName { get; set; }
            public object[] ProviderUserInfo { get; set; }
            public string PhotoUrl { get; set; }
            public string PasswordHash { get; set; }
            public double PasswordUpdatedAt { get; set; }
            public string ValidSince { get; set; }
            public bool Disabled { get; set; }
            public bool EmailVerified { get; set; }
            public string LastLoginAt { get; set; }
            public string CreatedAt { get; set; }
            public string CustomAuth { get; set; }


        }
    }
}
