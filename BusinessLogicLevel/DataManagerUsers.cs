using System;
using System.Collections.Generic;
using System.Linq;
using DataAccessLevel.Abstract;
using HolderArchiveApp.Domain.Entities;
using HolderArchiveApp.Service;

namespace BusinessLogicLevel
{
	public class DataManagerUsers : DataManagerBase
	{
		public DataManagerUsers(IPostgresRepository postgresRepository, IAcgdRepository acgdRepository, IIdentityServicesRepository identityRepository):
			base(postgresRepository, acgdRepository, identityRepository)
		{
		}

		public DataOperationResult GetUsersList(out List<User> listUsers)
		{
			try
			{
				listUsers = _identityRepository.Users.OrderBy(i => i.Id).ToList();
				listUsers = listUsers.Where(item => item.ContainsArchiveRole()).ToList();
				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				listUsers = null;
				ServiceMethods.LogException(ex, "Ошибка при попытке получить список пользователей (GetUsersList):", _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Не удалось получить данные. Попробуйте еще раз или обратитесь в службу технической поддержки"
				};
			}
		}

		public DataOperationResult GetLaboratoryForUser(string userId, out string lab)
		{
			lab = string.Empty;
			try
			{
				UserLaboratory userLab = _postgresRepository.GetUserLabById(userId);
				lab = userLab != null ? userLab.Laboratory : string.Empty;
				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				ServiceMethods.LogException(ex, "Ошибка при попытке получить лабораторию пользователя (GetLaboratoryForUser):", _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Не удалось получить лабораторию пользователя. Попробуйте еще раз или обратитесь в службу технической поддержки"
				};
			}
		}

		public DataOperationResult UpdateUserLaboratory(string userId, string lab)
		{
			try
			{
				UserLaboratory userLab = _postgresRepository.GetUserLabById(userId);
				if (userLab != null)
				{
					if (userLab.Laboratory != lab)
					{
						userLab.Laboratory = lab;
						_postgresRepository.Update(userLab);
					}
				}
				else  // if binding to laboratory doesn't exist
				{
					_postgresRepository.Add(new UserLaboratory
					{
						UserId = userId,
						Laboratory = lab
					});
				}
				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				string msg = $"Ошибка при попытке обновить лабораторию пользователя (UpdateUserLaboratory) с id: {userId}, лаборатория: {lab}";
				ServiceMethods.LogException(ex, msg, _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Ошибка при попытке обновить лабораторию пользователя"
				};
			}
		}
	}
}
