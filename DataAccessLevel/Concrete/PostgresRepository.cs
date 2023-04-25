using System;
using System.Drawing;
using System.Data.Entity;
using System.Linq;
using DataAccessLevel.Abstract;
using Helix.Logger;
using HolderArchiveApp.Domain;
using HolderArchiveApp.Domain.Entities;
using HolderArchiveApp.Service;

namespace DataAccessLevel.Concrete
{
	public sealed class PostgresRepository : GenericRepository, IPostgresRepository
	{
		protected override DbContext _context { get; set; }

		private static readonly HelixLogger _logger = HelixLogManager.GetCurrentClassLogger();

		public DbContext DbContext { get { return _context; } }

		public PostgresRepository()
		{
			_context = new PostgresDbContext();
			_context.Configuration.UseDatabaseNullSemantics = true;
		}

		public override void Dispose()
		{
			_context?.Dispose();
		}

		public IQueryable<HolderType> HolderTypes
		{
			get
			{
				var postgresDbContext = _context as PostgresDbContext;
				return postgresDbContext?.HolderTypes.Include(u => u.Holders);
			}
		}

		public IQueryable<Holder> Holders
		{
			get
			{
				var postgresDbContext = _context as PostgresDbContext;
				return postgresDbContext?.Holders;
			}
		}

		public IQueryable<WorkflowRecord> WorkflowRecords
		{
			get
			{
				var postgresDbContext = _context as PostgresDbContext;
				return postgresDbContext?.WorkflowRecords;

			}
		}

		public IQueryable<UserLaboratory> UserLabs
		{
			get
			{
				var postgresDbContext = _context as PostgresDbContext;
				return postgresDbContext?.UserLabs;
			}
		}

		public IQueryable<Sample> Samples
		{
			get
			{
				var postgresDbContext = _context as PostgresDbContext;
				return postgresDbContext?.Samples;
			}
		}

		public new T Update<T>(T entity, T attached = null) where T : class
		{
			if (attached != null && _context.Set<T>().Local.Any(e => e == attached))
			{
				var attachedEntry = _context.Entry<T>(attached);
				attachedEntry.CurrentValues.SetValues(entity);
				_context.SaveChanges();
				return attached;
			}

			if (entity is WorkflowRecord)
			{
				WorkflowRecord record = entity as WorkflowRecord;
				Update(record);
			}
			else if (entity is HolderType)
			{
				HolderType holderType = entity as HolderType;
				Update(holderType);
			}
			else return base.Update(entity, attached);

			return entity;
		}

		public void Update(WorkflowRecord record)
		{
			using (var transaction = _context.Database.BeginTransaction())
			{
				try
				{
					var postgresDbContext = _context as PostgresDbContext;
					if (postgresDbContext == null) throw new ApplicationException("Unable to get postgresDbContext");

					WorkflowRecord tmpRecord = postgresDbContext.WorkflowRecords
													.FirstOrDefault(r => r.Id == record.Id);

					if(tmpRecord == null) throw new ApplicationException("Unable to find edited row: WorkflowRecord");

					if (tmpRecord.Statuses != record.Statuses) tmpRecord.Statuses = record.Statuses;
					if (tmpRecord.Workflow != record.Workflow) tmpRecord.Workflow = record.Workflow;

					for (int i = 0; i < tmpRecord.ContainerTypes.Count; ++i)
					{
						ContainerTypeForWorkflow tmpContType =
							record.ContainerTypes.FirstOrDefault(ct => ct.Name == tmpRecord.ContainerTypes[i].Name);
						if (tmpContType == null)
						{
							_context.Entry(tmpRecord.ContainerTypes[i]).State = EntityState.Deleted;
							--i;   // the item is removed from collection and the next item will get its index
						}
					}

					foreach (var contType in record.ContainerTypes)
					{
						ContainerTypeForWorkflow tmpContType =
							tmpRecord.ContainerTypes.FirstOrDefault(ct => ct.Name == contType.Name);
						if (tmpContType == null)
						{
							ContainerTypeForWorkflow newContainerType = new ContainerTypeForWorkflow { Name = contType.Name, WorkflowRecord = tmpRecord};
							tmpRecord.ContainerTypes.Add(newContainerType);
							_context.Entry(newContainerType).State = EntityState.Added;
						}
					}

					postgresDbContext.Entry(tmpRecord).State = EntityState.Modified;
					postgresDbContext.SaveChanges();
					
					transaction.Commit();
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					ServiceMethods.LogException(ex, "Exception in PostgresRepository.Update(WorkflowRecord record):", _logger);
					throw;
				}
			}
		}

		public void Update(HolderType holderType)
		{
			using (var transaction = _context.Database.BeginTransaction())
			{
				try
				{
					var postgresDbContext = _context as PostgresDbContext;
					if (postgresDbContext == null) throw new ApplicationException("Unable to get postgresDbContext");

					HolderType tmpHolderType = postgresDbContext.HolderTypes
													.FirstOrDefault(r => r.Id == holderType.Id);

					if (tmpHolderType == null) throw new ApplicationException("Unable to find edited row: HolderType");

					if (tmpHolderType.Name != holderType.Name) tmpHolderType.Name = holderType.Name;
					if (tmpHolderType.LaboratoryName != holderType.LaboratoryName) tmpHolderType.LaboratoryName = holderType.LaboratoryName;
					if (tmpHolderType.HolderColor != holderType.HolderColor) tmpHolderType.HolderColor = holderType.HolderColor;
					if (tmpHolderType.CountCellsHorizontal != holderType.CountCellsHorizontal) tmpHolderType.CountCellsHorizontal = holderType.CountCellsHorizontal;
					if (tmpHolderType.CountCellsVertical != holderType.CountCellsVertical) tmpHolderType.CountCellsVertical = holderType.CountCellsVertical;
					if (tmpHolderType.CreatedOn != holderType.CreatedOn) tmpHolderType.CreatedOn = holderType.CreatedOn;
					if (tmpHolderType.ChangedOn != holderType.ChangedOn) tmpHolderType.ChangedOn = holderType.ChangedOn;

					for (int i = 0; i < tmpHolderType.ContainerTypes.Count; ++i)
					{
						ContainerTypeForHolderType tmpContType =
							holderType.ContainerTypes.FirstOrDefault(ct => ct.Name == tmpHolderType.ContainerTypes[i].Name);
						if (tmpContType == null)
						{
							_context.Entry(tmpHolderType.ContainerTypes[i]).State = EntityState.Deleted;
							--i;   // the item is removed from collection and the next item will get its index
						}
					}

					foreach (var contType in holderType.ContainerTypes)
					{
						ContainerTypeForHolderType tmpContType =
							tmpHolderType.ContainerTypes.FirstOrDefault(ct => ct.Name == contType.Name);
						if (tmpContType == null)
						{
							ContainerTypeForHolderType newContainerType = new ContainerTypeForHolderType { Name = contType.Name, HolderType = tmpHolderType };
							tmpHolderType.ContainerTypes.Add(newContainerType);
							_context.Entry(newContainerType).State = EntityState.Added;
						}
					}

					postgresDbContext.Entry(tmpHolderType).State = EntityState.Modified;
					postgresDbContext.SaveChanges();

					transaction.Commit();
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					ServiceMethods.LogException(ex, "Exception in PostgresRepository.Update(HolderType holderType):", _logger);
					throw;
				}
			}
		}

		public HolderType GetHolderTypeById(params object[] keyValues)
		{
			var postgresDbContext = _context as PostgresDbContext;
			return postgresDbContext?.HolderTypes.Find(keyValues);
		}

		public Holder GetHolderById(params object[] keyValues)
		{
			var postgresDbContext = _context as PostgresDbContext;
			return postgresDbContext?.Holders.Find(keyValues);
		}

		public WorkflowRecord GetWorkflowRecordById(params object[] keyValues)
		{
			var postgresDbContext = _context as PostgresDbContext;
			return postgresDbContext?.WorkflowRecords.Find(keyValues);
		}

		public UserLaboratory GetUserLabById(params object[] keyValues)
		{
			var postgresDbContext = _context as PostgresDbContext;
			return postgresDbContext?.UserLabs.Find(keyValues);
		}
	}
}
