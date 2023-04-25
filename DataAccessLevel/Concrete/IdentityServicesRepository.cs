using System.Data.Entity;
using System.Linq;
using DataAccessLevel.Abstract;
using HolderArchiveApp.Domain.Entities;

namespace DataAccessLevel.Concrete
{
	public class IdentityServicesRepository : GenericRepository, IIdentityServicesRepository
	{
		protected sealed override DbContext _context { get; set; }

		public IdentityServicesRepository()
		{
			_context = new IdentityServicesDbContext();
			_context.Configuration.UseDatabaseNullSemantics = true;
		}

		public override void Dispose()
		{
			_context?.Dispose();
		}

		public IQueryable<User> Users
		{
			get
			{
				var identityDbContext = _context as IdentityServicesDbContext;
				return identityDbContext?.Users;
			}
		}

		public User GetUserByName(string userName)
		{
			return Users.FirstOrDefault(u => u.Login == userName);
		}
	}
}
