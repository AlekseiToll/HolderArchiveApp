using System;
using System.Collections.Generic;
using System.Linq;
using DataAccessLevel.Abstract;
using HolderArchiveApp.Domain;
using HolderArchiveApp.Domain.Entities;
using HolderArchiveApp.Service;

namespace BusinessLogicLevel
{
	public class DataManagerResetHolder : DataManagerBase
	{
		public DataManagerResetHolder(IPostgresRepository postgresRepository, IAcgdRepository acgdRepository, IIdentityServicesRepository identityRepository):
			base(postgresRepository, acgdRepository, identityRepository)
		{
		}

		public DataOperationResult GetHoldersToReset(string userName, out List<HolderToReset> listHoldersToReset)
		{
			listHoldersToReset = new List<HolderToReset>();
			try
			{
				string curUserLab = GetUserLab(userName);
				var holderTypes = _postgresRepository.HolderTypes;
				foreach (var holderType in holderTypes)
				{
					// compare laboratory
					if (!string.IsNullOrEmpty(curUserLab) && curUserLab != holderType.LaboratoryName)
						continue;

					var holders = _postgresRepository.Holders.Where(h => h.HolderTypeId == holderType.Id).OrderBy(h => h.Id).ToList();
					foreach (Holder h in holders)
					{
						if (h.Status == EHolderStatus.ARCHIVED)
						{
							if (h.ArchivedOn != null)
							{
								var dt = h.ArchivedOn.Value.AddDays(holderType.TimeLimit);
								if (DateTime.Now.Date >= dt.Date)
								{
									listHoldersToReset.Add(new HolderToReset
									{
										Id = h.Id,
										HolderTypeName = holderType.Name,
										TimeLimit = holderType.TimeLimit,
										ArchivedOn = (DateTime)h.ArchivedOn
									});
								}
							}
							else
							{
								_logger.Error("Нет даты архивирования для штатива со статусом Архив с id = " + h.Id);
							}
						}
					}
				}

				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				string msg = "Ошибка при попытке получить список штативов для сброса (GetHoldersToReset)";
				ServiceMethods.LogException(ex, msg, _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Ошибка при попытке получить список штативов для сброса. Попробуйте еще раз или обратитесь в службу технической поддержки"
				};
			}
		}

		public DataOperationResult ResetHolder(int holderId)
		{
			try
			{
				var holders = _postgresRepository.Holders.Where(item => item.Id == holderId).ToList();
				if (holders.Any())
				{
					foreach (var holder in holders)
					{
						holder.Status = EHolderStatus.EMPTY;
						holder.ArchivedOn = null;
						_postgresRepository.Update(holder);
					}
				}
				else
				{
					return new DataOperationResult
					{
						ResultCode = DataOperationResult.ResultCodes.UnknownError,
						ErrorDescription = "Штатив не найден. Попробуйте еще раз или обратитесь в службу технической поддержки"
					};
				}

				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				string msg = $"Ошибка при попытке сбросить штатив {holderId} (ResetHolder):";
				ServiceMethods.LogException(ex, msg, _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = $"Ошибка при попытке сбросить штатив {holderId}"
				};
			}
		}

		public DataOperationResult GetColorForHolder(int holderId, out string color)
		{
			color = string.Empty;
			try
			{
				var holders = _postgresRepository.Holders.Where(h => h.Id == holderId).ToList();
				if (holders.Count < 0)
				{
					throw new TrackingException($"Штатив с id = {holderId} не найден в БД (GetColorForHolder)");
				}

				int holderTypeId = holders[0].HolderTypeId;
				var holderTypes = _postgresRepository.HolderTypes.Where(ht => ht.Id == holderTypeId).ToList();
				if (holderTypes.Count < 0)
				{
					throw new TrackingException($"Тип штатива с id = {holderTypeId} не найден в БД (GetColorForHolder)");
				}

				color = System.Drawing.ColorTranslator.ToHtml(holderTypes[0].HolderColor);
				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (TrackingException tex)
			{
				_logger.Error(tex.Message);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Ошибка при попытке получить цвет штатива. Попробуйте еще раз или обратитесь в службу технической поддержки"
				};
			}
			catch (Exception ex)
			{
				string msg = $"Ошибка при попытке получить цвет для штатива с id = {holderId} (GetColorForHolder):";
				ServiceMethods.LogException(ex, msg, _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Ошибка при попытке получить цвет штатива. Попробуйте еще раз или обратитесь в службу технической поддержки"
				};
			}
		}
	}
}
