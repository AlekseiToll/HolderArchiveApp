using System.Linq;
using HolderArchiveApp.Domain.Entities;

namespace DataAccessLevel.Abstract
{
	public interface IAcgdRepository : IGenericRepository
	{
		IQueryable<ContainerTypeACGD> ContainerTypes { get; }

		IQueryable<Laboratory> Labs { get; }

		IQueryable<Workflow> Workflows { get; }
	}
}
