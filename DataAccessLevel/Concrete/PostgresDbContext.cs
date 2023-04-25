using System.Data.Entity;
using HolderArchiveApp.Domain.Entities;

namespace DataAccessLevel.Concrete
{
	public class PostgresDbContext : DbContext
	{
		public PostgresDbContext() : base("PostgresDbContext")
		{
		}

		public DbSet<Holder> Holders { get; set; }

		public DbSet<WorkflowRecord> WorkflowRecords { get; set; }

		public DbSet<HolderType> HolderTypes { get; set; }

		public DbSet<UserLaboratory> UserLabs { get; set; }

		public DbSet<Sample> Samples { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			Database.SetInitializer<DbContext>(null);

			// Fluent API

			// PostgreSQL uses the public schema by default - not dbo.
			modelBuilder.HasDefaultSchema("public");

			modelBuilder.Entity<HolderType>().ToTable("holder_types");
			modelBuilder.Entity<HolderType>().HasKey(p => p.Id);		// Primary key
			modelBuilder.Entity<HolderType>().Property(p => p.Id).HasColumnName("id");
			modelBuilder.Entity<HolderType>().Property(p => p.Name).HasColumnName("type_name");
			modelBuilder.Entity<HolderType>().Property(p => p.HolderColorArgb).HasColumnName("color");
			modelBuilder.Entity<HolderType>().HasMany(p => p.ContainerTypes).WithOptional();
			modelBuilder.Entity<HolderType>().Property(p => p.CountCellsHorizontal).HasColumnName("cell_hor");
			modelBuilder.Entity<HolderType>().Property(p => p.CountCellsVertical).HasColumnName("cell_vert");
			modelBuilder.Entity<HolderType>().Property(p => p.TimeLimit).HasColumnName("time_limit");
			modelBuilder.Entity<HolderType>().Property(p => p.LaboratoryName).HasColumnName("lab");
			modelBuilder.Entity<HolderType>().Property(p => p.CreatedOn).HasColumnName("created_on");
			modelBuilder.Entity<HolderType>().Property(p => p.ChangedOn).HasColumnName("changed_on");

			modelBuilder.Entity<HolderType>()
				.HasMany(p => p.ContainerTypes)
				.WithRequired(p => p.HolderType)
				.Map(m => m.MapKey("holder_type_id"));

			modelBuilder.Entity<ContainerTypeForHolderType>().ToTable("holder_type__container_type");
			modelBuilder.Entity<ContainerTypeForHolderType>().Property(p => p.Name).HasColumnName("container_type");
			modelBuilder.Entity<ContainerTypeForHolderType>().Property(p => p.RecordId).HasColumnName("record_id");
			modelBuilder.Entity<ContainerTypeForHolderType>().HasKey(p => p.RecordId);     // Primary key

			modelBuilder.Entity<Holder>().ToTable("holders");
			modelBuilder.Entity<Holder>().HasKey(p => p.Id);     // Primary key
			modelBuilder.Entity<Holder>().Property(p => p.Id).HasColumnName("id");
			modelBuilder.Entity<Holder>().Property(p => p.HolderTypeId).HasColumnName("holder_type_id");
			modelBuilder.Entity<Holder>().Property(p => p.Status).HasColumnName("holder_status");
			modelBuilder.Entity<Holder>().Property(p => p.CreatedOn).HasColumnName("created_on");
			modelBuilder.Entity<Holder>().Property(p => p.DeletedOn).HasColumnName("deleted_on");
			modelBuilder.Entity<Holder>().Property(p => p.ArchivedOn).HasColumnName("archived_on");
			//=================================================================================================

			modelBuilder.Entity<WorkflowRecord>().ToTable("workflow_records");
			modelBuilder.Entity<WorkflowRecord>().Property(p => p.Id).HasColumnName("id");
			modelBuilder.Entity<WorkflowRecord>().Property(p => p.Workflow).HasColumnName("workflow_name");
			modelBuilder.Entity<WorkflowRecord>().Property(p => p.Statuses).HasColumnName("statuses");
			modelBuilder.Entity<WorkflowRecord>().HasMany(p => p.ContainerTypes).WithOptional();

			modelBuilder.Entity<WorkflowRecord>()
				.HasMany(p => p.ContainerTypes)
				.WithRequired(p => p.WorkflowRecord)
				.Map(m => m.MapKey("workflow_record_id"));

			modelBuilder.Entity<ContainerTypeForWorkflow>().ToTable("workflow_record__container_type");
			modelBuilder.Entity<ContainerTypeForWorkflow>().Property(p => p.Name).HasColumnName("container_type");
			modelBuilder.Entity<ContainerTypeForWorkflow>().Property(p => p.RecordId).HasColumnName("record_id");
			modelBuilder.Entity<ContainerTypeForWorkflow>().HasKey(p => p.RecordId);     // Primary key
			//=================================================================================================

			modelBuilder.Entity<UserLaboratory>().ToTable("users");
			modelBuilder.Entity<UserLaboratory>().HasKey(p => p.UserId);     // Primary key
			modelBuilder.Entity<UserLaboratory>().Property(p => p.UserId).HasColumnName("user_id");
			modelBuilder.Entity<UserLaboratory>().Property(p => p.Laboratory).HasColumnName("lab");
			//=================================================================================================

			modelBuilder.Entity<Sample>().ToTable("samples");
			modelBuilder.Entity<Sample>().HasKey(p => p.RecordId);     // Primary key
			modelBuilder.Entity<Sample>().Property(p => p.RecordId).HasColumnName("record_id");
			modelBuilder.Entity<Sample>().Property(p => p.SampleNumber).HasColumnName("sample_number");
			modelBuilder.Entity<Sample>().Property(p => p.LabelId).HasColumnName("label_id");
			modelBuilder.Entity<Sample>().Property(p => p.Status).HasColumnName("status");
			modelBuilder.Entity<Sample>().Property(p => p.Workflow).HasColumnName("x_workflow");
			modelBuilder.Entity<Sample>().Property(p => p.ContainerType).HasColumnName("container_type");
			modelBuilder.Entity<Sample>().Property(p => p.SampleTemplate).HasColumnName("sample_template");
			modelBuilder.Entity<Sample>().Property(p => p.ChangedOn).HasColumnName("changed_on");
			modelBuilder.Entity<Sample>().Property(p => p.LoginOn).HasColumnName("login_on");
			modelBuilder.Entity<Sample>().Property(p => p.HolderId).HasColumnName("holder_id");
			modelBuilder.Entity<Sample>().Property(p => p.Row).HasColumnName("holder_row");
			modelBuilder.Entity<Sample>().Property(p => p.Column).HasColumnName("holder_column");

			base.OnModelCreating(modelBuilder);
		}
	}
}
