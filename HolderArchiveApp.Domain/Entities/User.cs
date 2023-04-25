using System;
using System.Collections.Generic;
using System.Linq;

namespace HolderArchiveApp.Domain.Entities
{
	// from HelixIdentity.AspNetUsers
	public class User
	{
		public User()
		{
			UserRoles = new List<UserRole>();
		}

		public string Id { get; set; }
		public string Login { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }

		public virtual IList<UserRole> UserRoles { get; set; }

		public bool ContainsRole(string role)
		{
			return UserRoles.Any(item => item.Role == role);
		}

		public bool ContainsArchiveRole()
		{
			foreach (EUserRole role in Enum.GetValues(typeof (EUserRole)))
			{
				if (UserRoles.Any(item => string.Equals(item.Role, role.ToString(), StringComparison.OrdinalIgnoreCase)))
						return true;
			}
			return false;
		}
	}

	// from HelixIdentity.AspNetUserClaims
	public class UserRole
	{
		public int Id { get; set; }
		public string Role { get; set; }

		public virtual User User { get; set; }
	}

	// from holder_archive_db.users (PostgreSQL)
	public class UserLaboratory
	{
		//public int RecordId { get; set; }
		public string UserId { get; set; }
		public string Laboratory { get; set; }
	}
}
