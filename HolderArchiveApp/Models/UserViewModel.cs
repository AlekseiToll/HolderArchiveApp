using HolderArchiveApp.Domain.Entities;

namespace HolderArchiveApp.Models
{
	public class UserViewModel
	{
		public string Id { get; set; }
		public string Login { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string LaboratoryName { get; set; }
		//public string Role { get; set; }

		public UserViewModel()
		{
		}

		public UserViewModel(User dataModel, string laboratoryName)
		{
			Id = dataModel.Id;
			Login = dataModel.Login;
			FirstName = dataModel.FirstName;
			LastName = dataModel.LastName;
			LaboratoryName = laboratoryName;
			//Role = role;
		}

		public User GetUser()
		{
			User user = new User
			{
				Id = Id,
				Login = Login,
				FirstName = FirstName,
				LastName = LastName
			};
			return user;
		}
	}
}