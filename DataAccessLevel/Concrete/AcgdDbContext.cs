using System.Data.Entity;
using HolderArchiveApp.Domain.Entities;

namespace DataAccessLevel.Concrete
{
	public class AcgdDbContext : DbContext
	{
		public AcgdDbContext() : base("AcgdDbContext")
		{
		}

		public DbSet<ContainerTypeACGD> ContainerTypes { get; set; }

		public DbSet<Laboratory> Labs { get; set; }

		public DbSet<Workflow> Workflows { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			// Fluent API

			modelBuilder.HasDefaultSchema("dbo");

			modelBuilder.Entity<ContainerTypeACGD>().ToTable("AllContainerTypes");
			modelBuilder.Entity<ContainerTypeACGD>().HasKey(p => p.Name);		// Primary key
			modelBuilder.Entity<ContainerTypeACGD>().Property(p => p.Name).HasColumnName("NAME");
			modelBuilder.Entity<ContainerTypeACGD>().Property(p => p.Description).HasColumnName("DESCRIPTION");
			modelBuilder.Entity<ContainerTypeACGD>().Property(p => p.GroupName).HasColumnName("GROUP_NAME");
			modelBuilder.Entity<ContainerTypeACGD>().Property(p => p.ChangedBy).HasColumnName("CHANGED_BY");
			modelBuilder.Entity<ContainerTypeACGD>().Property(p => p.ChangedOn).HasColumnName("CHANGED_ON");
			modelBuilder.Entity<ContainerTypeACGD>().Property(p => p.IsRemoved).HasColumnName("IsRemoved");

			modelBuilder.Entity<Laboratory>().ToTable("AllHubs");
			modelBuilder.Entity<Laboratory>().HasKey(p => p.Code);		// Primary key
			modelBuilder.Entity<Laboratory>().Property(p => p.Code).HasColumnName("Code");
			modelBuilder.Entity<Laboratory>().Property(p => p.Description).HasColumnName("Description");
			modelBuilder.Entity<Laboratory>().Property(p => p.IsRemoved).HasColumnName("IsRemoved");
			modelBuilder.Entity<Laboratory>().Property(p => p.TimeStamp).HasColumnName("TimeStamp");

			modelBuilder.Entity<Workflow>().ToTable("AllWorkflows");
			modelBuilder.Entity<Workflow>().HasKey(p => p.Code);		// Primary key
			modelBuilder.Entity<Workflow>().Property(p => p.Code).HasColumnName("Code");
			modelBuilder.Entity<Workflow>().Property(p => p.Description).HasColumnName("Description");
			modelBuilder.Entity<Workflow>().Property(p => p.IsRemoved).HasColumnName("IsRemoved");

			base.OnModelCreating(modelBuilder);
		}
	}
}
