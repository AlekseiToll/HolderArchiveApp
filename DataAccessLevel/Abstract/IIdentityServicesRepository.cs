using System.Linq;
using HolderArchiveApp.Domain.Entities;

namespace DataAccessLevel.Abstract
{
	public interface IIdentityServicesRepository : IGenericRepository
	{
		IQueryable<User> Users { get; }

		User GetUserByName(string userName);
	}
}
