using System;
using System.Collections.Generic;
using System.Text;

namespace CUnity.Shared.Models
{
    public class ApiLog
    {
        public virtual long Id { get; set; }

        public DateTime RequestTime { get; set; }

        public long ResponseMillis { get; set; }

        public int StatusCode { get; set; }

        public string Method { get; set; }

        public string Path { get; set; }

        public string QueryString { get; set; }

        public string RequestBody { get; set; }

        public string ResponseBody { get; set; }

        public string IPAddress { get; set; }

        public Guid? ApplicationUserId { get; set; }
    }
    public class ErrorLog : BaseEntity
    {
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public string ErrorType { get; set; }
        public string ErrorSource { get; set; }
        public DateTime ErrorDate { get; set; }
    }
}
