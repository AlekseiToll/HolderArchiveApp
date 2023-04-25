using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLevel.Abstract;
using HolderArchiveApp.Domain.Entities;
using HolderArchiveApp.Service;

namespace BusinessLogicLevel
{
	public class DataManagerService : DataManagerBase
	{
		public DataManagerService(IPostgresRepository postgresRepository, IAcgdRepository acgdRepository, IIdentityServicesRepository identityRepository):
			base(postgresRepository, acgdRepository, identityRepository)
		{
		}

		public DataOperationResult GetLabs(out IQueryable<Laboratory> listLabs)
		{
			listLabs = null;
			try
			{
				listLabs = _acgdRepository.Labs;
				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				ServiceMethods.LogException(ex, "Ошибка при попытке получить список лабораторий (GetLabs):", _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Не удалось получить список лабораторий. Попробуйте еще раз или обратитесь в службу технической поддержки"
				};
			}
		}

		public DataOperationResult GetContainerTypes(out IQueryable<ContainerTypeACGD> listContainerTypes)
		{
			listContainerTypes = null;
			try
			{
				listContainerTypes = _acgdRepository.ContainerTypes;
				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				ServiceMethods.LogException(ex, "Ошибка при попытке получить список типов контейнеров (GetContainerTypes):", _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Не удалось получить список типов контейнеров. Попробуйте еще раз или обратитесь в службу технической поддержки"
				};
			}
		}
	}
}
