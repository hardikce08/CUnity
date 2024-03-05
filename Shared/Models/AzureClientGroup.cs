using System;
using System.Collections.Generic;
using System.Text;

namespace CUnity.Shared.Models
{
    public class AzureClientGroup:BaseEntity
    {
        public int ClientId { get; set; }

        public string AzureGroupName { get; set; }
        public string AzureObjectId { get; set; }

        public string PowerBIWorkSpaceId { get; set; }

        public bool IsReportCopied { get; set; }
    }
    public class AzureUserList
    { 
        public string DisplayName { get; set; }
        public string GivenName { get; set; }
        public string FirstName { get; set; }
        public string AzureObjectId { get; set; }
        public string AzureGroupId { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; } 
        public string Password { get; set; }
        public string NickName { get; set; }
    }

     

}
