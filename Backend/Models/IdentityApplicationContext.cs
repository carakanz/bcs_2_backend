using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class IdentityApplicationContext : IdentityDbContext<DBUser>
    {
        public DbSet<DBUserBond> UserBonds { get; set; }
        public DbSet<DBBond> Bonds { get; set; }
        public DbSet<ViewModels.Company> Companies {get;set;}
        public IdentityApplicationContext(DbContextOptions<IdentityApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
