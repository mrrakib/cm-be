using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Models
{
    public class identity_dbcontext : IdentityDbContext<ApplicationUser, IdentityRole<long>, long>
    {
        public identity_dbcontext(DbContextOptions<identity_dbcontext> options)
            : base(options)
        {
        }

        public DbSet<Organization> Organizations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().ToTable("users");
            modelBuilder.Entity<IdentityRole<long>>().ToTable("roles");
            modelBuilder.Entity<IdentityUserRole<long>>().ToTable("user_roles");
            modelBuilder.Entity<IdentityUserClaim<long>>().ToTable("user_claims");
            modelBuilder.Entity<IdentityUserLogin<long>>().ToTable("user_logins");
            modelBuilder.Entity<IdentityUserToken<long>>().ToTable("user_tokens");
            modelBuilder.Entity<IdentityRoleClaim<long>>().ToTable("role_claims");
            modelBuilder.Entity<Organization>().ToTable("organizations");
        }
    }
}
