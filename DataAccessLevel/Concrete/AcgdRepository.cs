using System.Data.Entity;
using System.Linq;
using DataAccessLevel.Abstract;
using HolderArchiveApp.Domain.Entities;

namespace DataAccessLevel.Concrete
{
	public sealed class AcgdRepository : GenericRepository, IAcgdRepository
	{
		protected override DbContext _context { get; set; }

		public AcgdRepository()
		{
			_context = new AcgdDbContext();
			_context.Configuration.UseDatabaseNullSemantics = true;
		}

		public override void Dispose()
		{
			_context?.Dispose();
		}

		public IQueryable<ContainerTypeACGD> ContainerTypes
		{
			get
			{
				var acgdDbContext = _context as AcgdDbContext;
				return acgdDbContext?.ContainerTypes.Where(ct => ct.IsRemoved == false);
			}
		}

		public IQueryable<Laboratory> Labs
		{
			get
			{
				var acgdDbContext = _context as AcgdDbContext;
				return acgdDbContext?.Labs;
			}
		}

		public IQueryable<Workflow> Workflows
		{
			get
			{
				var acgdDbContext = _context as AcgdDbContext;
				return acgdDbContext?.Workflows.Where(w => w.IsRemoved == false);
			}
		}
	}
}
