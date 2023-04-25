using System;
using System.Collections.Generic;
using System.Linq;
using DataAccessLevel.Abstract;
using HolderArchiveApp.Domain;
using HolderArchiveApp.Domain.Entities;
using HolderArchiveApp.Service;

namespace BusinessLogicLevel
{
	public class DataManagerHolderTypes : DataManagerBase
	{
		public DataManagerHolderTypes(IPostgresRepository postgresRepository, IAcgdRepository acgdRepository, IIdentityServicesRepository identityRepository):
			base(postgresRepository, acgdRepository, identityRepository)
		{
		}

		public DataOperationResult GetHolderTypesList(out List<HolderType> listHolderTypes)
		{
			try
			{
				listHolderTypes = _postgresRepository.HolderTypes.OrderBy(i => i.Id).ToList();
				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				listHolderTypes = null;
				ServiceMethods.LogException(ex, "Ошибка при попытке получить список типов штативов (GetHolderTypesList):", _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Не удалось получить данные. Попробуйте еще раз или обратитесь в службу технической поддержки"
				};
			}
		}

		public DataOperationResult GetHoldersByType(int holderTypeId, out List<Holder> listHolders)
		{
			try
			{
				listHolders = _postgresRepository.Holders.Where(h => h.HolderTypeId == holderTypeId).OrderBy(h => h.Id).ToList();
				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				listHolders = null;
				string msg = $"Ошибка при попытке получить список штативов для типа штатива с id = {holderTypeId} (GetHoldersByType):";
				ServiceMethods.LogException(ex, msg, _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Ошибка при попытке получить список штативов. Попробуйте еще раз или обратитесь в службу технической поддержки"
				};
			}
		}

		public DataOperationResult GetCountNewHoldersForHolderType(int holderTypeId, out int count)
		{
			try
			{
				count = _postgresRepository.Holders.Where(h => h.HolderTypeId == holderTypeId && h.Status == EHolderStatus.NEW).ToList().Count;
				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				count = -1;
				string msg = $"Ошибка при попытке получить кол-во новых штативов для типа штатива с id = {holderTypeId} (GetCountNewHoldersForHolderType):";
				ServiceMethods.LogException(ex, msg, _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Ошибка при попытке получить количество новых штативов. Попробуйте еще раз или обратитесь в службу технической поддержки"
				};
			}
		}

		public DataOperationResult GetNewHoldersForHolderType(int holderTypeId, out int[] newHoldersIds)
		{
			try
			{
				var holders = _postgresRepository.Holders.Where(h => h.HolderTypeId == holderTypeId && h.Status == EHolderStatus.NEW).ToList();
				newHoldersIds = holders.Select(h => h.Id).ToArray();
				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				newHoldersIds = null;
				string msg = $"Ошибка при попытке получить новые штативы для типа штатива с id = {holderTypeId} (GetNewHoldersForHolderType):";
				ServiceMethods.LogException(ex, msg, _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Ошибка при попытке получить список новых штативов. Попробуйте еще раз или обратитесь в службу технической поддержки"
				};
			}
		}

		public DataOperationResult AddHolderType(HolderType holderType)
		{
			try
			{
				if (holderType.Name.Length > 30)
				{
					return new DataOperationResult
					{
						ResultCode = DataOperationResult.ResultCodes.ValidationError,
						ErrorDescription = "Не удалось сохранить. Значение в поле Тип штатива не должно быть более 30 символов"
					};
				}

				_postgresRepository.Add(holderType);
				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				string msg = "Ошибка при попытке добавить тип штатива (AddHolderType). Объект: " + ServiceMethods.ObjectToJson(holderType);
				ServiceMethods.LogException(ex, msg, _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Ошибка при попытке добавить тип штатива"
				};
			}
		}

		public DataOperationResult UpdateHolderType(HolderType holderType)
		{
			try
			{
				if (holderType.Name.Length > 30)
				{
					return new DataOperationResult
					{
						ResultCode = DataOperationResult.ResultCodes.ValidationError,
						ErrorDescription = "Не удалось обновить. Значение в поле Тип штатива не должно быть более 30 символов"
					};
				}

				_postgresRepository.Update(holderType);
				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				string msg = "Ошибка при попытке обновить тип штатива (UpdateHolderType). Объект: " + ServiceMethods.ObjectToJson(holderType);
				ServiceMethods.LogException(ex, msg, _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Ошибка при попытке обновить тип штатива"
				};
			}
		}

		public DataOperationResult DeleteHolderConfirmed(int id)
		{
			try
			{
				Holder holder = _postgresRepository.GetHolderById(id);
				_postgresRepository.Remove(holder);
				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				string msg = "Ошибка при попытке удалить штатив (DeleteHolderConfirmed) с id " + id;
				ServiceMethods.LogException(ex, msg, _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Ошибка при попытке удалить штатив"
				};
			}
		}

		public DataOperationResult AllVirtualHoldersToWork(int holderTypeId)
		{
			try
			{
				var holders = _postgresRepository.Holders.Where(item => item.HolderTypeId == holderTypeId && item.Status == EHolderStatus.NEW).ToList();
				if (holders.Any())
				{
					foreach (var holder in holders)
					{
						holder.Status = EHolderStatus.EMPTY;
						_postgresRepository.Update(holder);
					}
				}
				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				string msg = "Ошибка при попытке перевести в пустые все новые штативы (AllVirtualHoldersToWork) для типа с id " + holderTypeId;
				ServiceMethods.LogException(ex, msg, _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Ошибка при попытке перевести в пустые все новые штативы"
				};
			}
		}

		public DataOperationResult OneVirtualHolderToWork(int holderId)
		{
			try
			{
				var holders = _postgresRepository.Holders.Where(item => item.Id == holderId && item.Status == EHolderStatus.NEW).ToList();
				if (holders.Any())
				{
					foreach (var holder in holders)
					{
						holder.Status = EHolderStatus.EMPTY;
						_postgresRepository.Update(holder);
					}
				}
				else
				{
					return new DataOperationResult
					{
						ResultCode = DataOperationResult.ResultCodes.UnknownError,
						ErrorDescription = "Штатив не найден или его статус не Новый. Попробуйте еще раз или обратитесь в службу технической поддержки"
					};
				}
				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				string msg = "Ошибка при попытке перевести в пустые штатив (OneVirtualHolderToWork) с id " + holderId;
				ServiceMethods.LogException(ex, msg, _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Ошибка при попытке перевести в пустые один штатив"
				};
			}
		}

		public DataOperationResult AddHoldersToType(int holderTypeId, int count)
		{
			try
			{
				for (var iHolder = 0; iHolder < count; ++iHolder)
				{
					var holder = new Holder
					{
						CreatedOn = DateTime.Now,
						HolderTypeId = holderTypeId,
						Status = EHolderStatus.NEW
					};
					_postgresRepository.Add(holder);
				}
				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				string msg = $"Ошибка при попытке добавить {count} штативов к типу штатива с id = {holderTypeId} (AddHoldersToType):";
				ServiceMethods.LogException(ex, msg, _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = $"Ошибка при попытке добавить { count } штативов к типу штатива с id = { holderTypeId }"
				};
			}
		}
	}
}
