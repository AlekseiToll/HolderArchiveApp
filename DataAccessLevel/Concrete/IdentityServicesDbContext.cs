using System.Data.Entity;
using HolderArchiveApp.Domain.Entities;

namespace DataAccessLevel.Concrete
{
	public class IdentityServicesDbContext : DbContext
	{
		public IdentityServicesDbContext() : base("IdentityServicesDbContext")
		{
		}
		public DbSet<User> Users { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			// Fluent API
			modelBuilder.HasDefaultSchema("dbo");

			modelBuilder.Entity<User>().ToTable("AspNetUsers");

			modelBuilder.Entity<User>().HasKey(p => p.Id);		// Primary key
			modelBuilder.Entity<User>().Property(p => p.Id).HasColumnName("Id");
			modelBuilder.Entity<User>().Property(p => p.Login).HasColumnName("UserName");
			modelBuilder.Entity<User>().Property(p => p.LastName).HasColumnName("LastName");
			modelBuilder.Entity<User>().Property(p => p.FirstName).HasColumnName("FirstName");
			modelBuilder.Entity<User>()
				.HasMany(p => p.UserRoles)
				.WithRequired(p => p.User)
				.Map(m => m.MapKey("UserId"));

			modelBuilder.Entity<UserRole>().ToTable("AspNetUserClaims");
			modelBuilder.Entity<UserRole>().Property(p => p.Role).HasColumnName("ClaimValue");
			modelBuilder.Entity<UserRole>().HasKey(p => p.Id);     // Primary key
			modelBuilder.Entity<UserRole>().Property(p => p.Id).HasColumnName("Id");

			base.OnModelCreating(modelBuilder);
		}
	}
}
