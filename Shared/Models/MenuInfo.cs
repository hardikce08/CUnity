using System;
using System.Collections.Generic;
using System.Text;

namespace CUnity.Shared.Models
{
    public class MenuInfo : BaseEntity
    {
        //public int MenuId { get; set; }
        public int ParentMenuId { get; set; }
        public string PageName { get; set; }
        public string MenuName { get; set; }
        public string IconName { get; set; }
    }
}
