using System;
using System.Collections.Generic;
using System.Text;

namespace CUnity.Shared.Models
{
    public class AzureSetting : BaseEntity
    {
        public string GroupName { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
