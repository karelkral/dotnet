using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using brechtbaekelandt.ldap.Identity.Models;

namespace brechtbaekelandt.ldap.ViewModels
{
    public class ManageViewModel
    {
        public ICollection<LdapUser> Users { get; set; }

        public LdapUser NewUser { get; set; }
    }
}
