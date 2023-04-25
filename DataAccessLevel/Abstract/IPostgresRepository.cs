using System.Data.Entity;
using System.Linq;
using HolderArchiveApp.Domain.Entities;

namespace DataAccessLevel.Abstract
{
	public interface IPostgresRepository : IGenericRepository
	{
		DbContext DbContext { get; }

		IQueryable<HolderType> HolderTypes { get; }
		IQueryable<Holder> Holders { get; }
		IQueryable<WorkflowRecord> WorkflowRecords { get; }
		IQueryable<UserLaboratory> UserLabs { get; }
		IQueryable<Sample> Samples { get; }

		HolderType GetHolderTypeById(params object[] keyValues);
		Holder GetHolderById(params object[] keyValues);
		WorkflowRecord GetWorkflowRecordById(params object[] keyValues);
		UserLaboratory GetUserLabById(params object[] keyValues);
	}
}
