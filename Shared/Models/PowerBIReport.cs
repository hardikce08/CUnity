using System;
using System.Collections.Generic;
using System.Text;

namespace CUnity.Shared.Models
{
    public class PowerBIReport : BaseEntity
    {
        public string ReportType { get; set; }
        public string ReportName { get; set; }
        public string ReportDisplayName { get; set; }
        public string ReportGuid { get; set; }
    }
}
