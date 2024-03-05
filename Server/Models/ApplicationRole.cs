using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CUnity.Server.Models
{
    public class ApplicationRole : IdentityRole
    {
        //public ApplicationRole() : base() { }
        //public ApplicationRole(string roleName) : base(roleName) { }
        //public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
