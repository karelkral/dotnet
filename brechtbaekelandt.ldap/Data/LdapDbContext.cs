using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using brechtbaekelandt.ldap.Identity.Models;

namespace brechtbaekelandt.ldap.Data
{
    public class LdapDbContext : IdentityDbContext<LdapUser>
    {
        public LdapDbContext(DbContextOptions<LdapDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
