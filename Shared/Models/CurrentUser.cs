using System;
using System.Collections.Generic;
using System.Text;

namespace CUnity.Shared.Models
{
    public class CurrentUser
    {
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
        public Dictionary<string, string> Claims { get; set; }
        public int? UserId { get; set; }
        public string UserGuid { get; set; }

        public bool HasOpticsAccess { get; set; }
        public bool IsAdmin { get; set; }
        public int? ClientId { get; set; }
        public bool IsSuperAdmin { get; set; }
    }

    public class UserViewModel  
    {
        public bool IsAuthenticated { get; set; }
        public string UserGuId { get; set; }
        public string UserName { get; set; }
        //public string TenantId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public bool HasPassword { get; set; }
        //public string PhoneNumber { get; set; }
        //public bool TwoFactorEnabled { get; set; }
        //public bool HasAuthenticator { get; set; }
       // public List<KeyValuePair<string, string>> Logins { get; set; }
        //public bool BrowserRemembered { get; set; }
        //public string SharedKey { get; set; }
        //public string AuthenticatorUri { get; set; }
        //public string[] RecoveryCodes { get; set; }
        //public int CountRecoveryCodes { get; set; }
        public List<string> Roles { get; set; }
        //public List<KeyValuePair<string, string>> ExposedClaims { get; set; }
    }
}
