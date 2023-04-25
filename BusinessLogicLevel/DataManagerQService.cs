using System;
using System.Linq;
using DataAccessLevel.Abstract;
using HolderArchiveApp.Domain.Entities;
using HolderArchiveApp.Service;

namespace BusinessLogicLevel
{
	public class DataManagerQService : DataManagerBase
	{
		public DataManagerQService(IPostgresRepository postgresRepository, IAcgdRepository acgdRepository, IIdentityServicesRepository identityRepository):
			base(postgresRepository, acgdRepository, identityRepository)
		{
		}

		public DataOperationResult AddOrUpdateSample(Sample sample)
		{
			try
			{
				var samples = _postgresRepository.Samples.Where(item => item.SampleNumber == sample.SampleNumber).ToList();
				if (samples.Any())
				{
					if (samples[0].ChangedOn < sample.ChangedOn)  // если даже статус не менялся, надо обновить дату
					{
						samples[0].Status = sample.Status;
						samples[0].ChangedOn = sample.ChangedOn;
						_postgresRepository.Update(samples[0]);
					}
				}
				else
				{
					_postgresRepository.Add(sample);
				}
				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				string msg = "Ошибка при попытке добавить данные о пробе. Объект: " + ServiceMethods.ObjectToJson(sample);
				ServiceMethods.LogException(ex, msg, _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Ошибка при попытке добавить данные о пробе"
				};
			}
		}
	}
}
