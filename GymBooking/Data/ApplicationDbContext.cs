using System;
using System.Collections.Generic;
using System.Text;
using GymBooking.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GymBooking.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUserGymClass>().HasKey(t => new { t.ApplicationUserId, t.GymClassId });
        }

        public DbSet<GymBooking.Models.GymClass> GymClass { get; set; }

        public DbSet<GymBooking.Models.ApplicationUserGymClass> ApplicationUserGymClass { get; set; }

    }
}
