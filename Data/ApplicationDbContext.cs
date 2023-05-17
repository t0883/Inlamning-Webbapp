using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Inlamning_Webbapp.Models;
using System.Security.Claims;

namespace Inlamning_Webbapp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Inlamning_Webbapp.Models.Actor> Actor { get; set; } = default!;
        public DbSet<Inlamning_Webbapp.Models.Movie> Movie { get; set; } = default!;
        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Actor>().Property(Price => Price.property).HasPrecision(18, 2);

            base.OnModelCreating(modelBuilder);
        }
    }
}