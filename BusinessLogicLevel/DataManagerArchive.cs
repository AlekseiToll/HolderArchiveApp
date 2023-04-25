using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DataAccessLevel.Abstract;
using HolderArchiveApp.Domain;
using HolderArchiveApp.Domain.Entities;
using HolderArchiveApp.Service;

namespace BusinessLogicLevel
{
	public class DataManagerArchive : DataManagerBase
	{
		public DataManagerArchive(IPostgresRepository postgresRepository, IAcgdRepository acgdRepository, IIdentityServicesRepository identityRepository):
			base(postgresRepository, acgdRepository, identityRepository)
		{
		}

		public DataOperationResult GetHoldersForArchive(string userName, out List<HolderTypeToArchive> listHolderTypesToArchive)
		{
			listHolderTypesToArchive = new List<HolderTypeToArchive>();
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
					int idFirstEmptyHolder = -1; // useful only if there is no archived holder or if the holder with max id is archived
					int idFirstEmptyHolderAfterArchived = -1;
					foreach (Holder h in holders)
					{
						if (h.Status == EHolderStatus.ARCHIVED)
						{
							idFirstEmptyHolderAfterArchived = -1; // reset when we meet an arcived holder
						}
						if (idFirstEmptyHolder == -1)
						{
							if (h.Status == EHolderStatus.EMPTY) idFirstEmptyHolder = h.Id;
						}
						if (idFirstEmptyHolderAfterArchived == -1)
						{
							if (h.Status == EHolderStatus.EMPTY) idFirstEmptyHolderAfterArchived = h.Id;
						}
					}

					int holderId = (idFirstEmptyHolderAfterArchived != -1) ? idFirstEmptyHolderAfterArchived : idFirstEmptyHolder;
					string info = string.Empty;
					if (holderId < 0)
					{
						info = HolderToResetExists(holderType.Id) ? "Нужен сброс" : "Нужно добавить";
					}

					listHolderTypesToArchive.Add(new HolderTypeToArchive
					{
						Id = holderType.Id,
						Name = holderType.Name,
						Color = ColorTranslator.ToHtml(holderType.HolderColor),
						NextHolderToArchive = holderId,
						Info = info
					});
				}

				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				string msg = "Ошибка при попытке получить список штативов для архивации (GetHoldersForArchive)";
				ServiceMethods.LogException(ex, msg, _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Ошибка при попытке получить список штативов для архивации. Попробуйте еще раз или обратитесь в службу технической поддержки"
				};
			}
		}

		private bool HolderToResetExists(int holderTypeId)
		{
			try
			{
				var holderType = _postgresRepository.GetHolderTypeById(holderTypeId);
				var holders = _postgresRepository.Holders.Where(h => h.HolderTypeId == holderTypeId).OrderBy(h => h.Id).ToList();
				foreach (Holder h in holders)
				{
					if (h.Status == EHolderStatus.ARCHIVED)
					{
						if (h.ArchivedOn == null)
						{
							_logger.Error("В БД нет даты архивации для архивированного штатива");
						}
						else
						{
							var dt = h.ArchivedOn.Value.AddDays(holderType.TimeLimit);
							if (DateTime.Now.Date >= dt.Date)
								return true;
						}
					}
				}
				return false;
			}
			catch (Exception ex)
			{
				string msg = $"Ошибка при определении наличия штатива для сброса, тип штатива с id = {holderTypeId} (HolderToResetExists):";
				ServiceMethods.LogException(ex, msg, _logger);
				throw;
			}
		}

		public DataOperationResult GetHolderTypeById(int holderTypeId, out HolderType holderType)
		{
			holderType = null;
			try
			{
				holderType = _postgresRepository.GetHolderTypeById(holderTypeId);
				if (holderType == null)
				{
					throw new TrackingException($"Тип штатива с id = {holderTypeId} не найден в БД (GetHolderTypeById)");
				}
				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				string msg = $"Ошибка при попытке получить тип штатива с id = {holderTypeId} (GetHolderTypeById):";
				ServiceMethods.LogException(ex, msg, _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Ошибка при попытке получить тип штатива. Попробуйте еще раз или обратитесь в службу технической поддержки"
				};
			}
		}

		public DataOperationResult GetValidityOfSample(string barCode, int holderTypeId, out bool res, out string info)
		{
			res = false;
			info = string.Empty;
			try
			{
				Sample curSample = GetSampleByBarCode(barCode);
				if (curSample == null)
				{
					return new DataOperationResult
					{
						ResultCode = DataOperationResult.ResultCodes.UnknownError,
						ErrorDescription = "Не найдены данные о пробе"
					};
				}

				HolderType curHolderType = _postgresRepository.GetHolderTypeById(holderTypeId);
				if (curHolderType == null)
				{
					return new DataOperationResult
					{
						ResultCode = DataOperationResult.ResultCodes.UnknownError,
						ErrorDescription = "Не найдены данные о типе штатива"
					};
				}

				if (curSample.SampleTemplate != "smp_in")
				{
					res = false;
					info = "sample.template не равен 'smp_in'";
					return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
				}

				bool containerTypeValid = false;
				foreach (var containerType in curHolderType.ContainerTypes)
				{
					if (containerType.Name == curSample.ContainerType)
						containerTypeValid = true;
				}
				if (!containerTypeValid)
				{
					res = false;
					info = "Тип контейнера не привязан к типу штатива";
					return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
				}

				WorkflowRecord curWorkflowRecord = null;
				IQueryable<WorkflowRecord> workflowRecords = _postgresRepository.WorkflowRecords;
				foreach (var workflowRecord in workflowRecords)
				{
					if (workflowRecord.Workflow == curSample.Workflow)
						curWorkflowRecord = workflowRecord;
				}
				if (curWorkflowRecord == null)
				{
					if (curSample.Status == "A" || curSample.Status == "X")
					{
						res = true;
						return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
					}
					else
					{
						res = false;
						info = "Нет записи о рабочем потоке и статус пробы не равен А или Х";
						return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
					}
				}

				containerTypeValid = false;
				foreach (var containerType in curWorkflowRecord.ContainerTypes)
				{
					if (containerType.Name == curSample.ContainerType)
						containerTypeValid = true;
				}
				if (!containerTypeValid)
				{
					if (curSample.Status == "A" || curSample.Status == "X")
					{
						res = true;
						return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
					}
					else
					{
						res = false;
						info = "Нет записи о рабочем потоке и статус пробы не равен А или Х";
						return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
					}
				}

				bool statusValid = false;
				List<string> workflowRecordStatuses = curWorkflowRecord.GetStatusesAsStrings();
				foreach (var status in workflowRecordStatuses)
				{
					if (status == curSample.ContainerType)
						statusValid = true;
				}
				if (!statusValid)
				{
					res = false;
					info = "Статус пробы не совпадает со статусом рабочего потока";
					return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
				}

				res = true;
				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				res = false;
				string msg = $"Ошибка при проверке валидности пробы со штрих-кодом {barCode} (GetValidityOfSample):";
				ServiceMethods.LogException(ex, msg, _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Ошибка при проверке валидности пробы. Попробуйте еще раз или обратитесь в службу технической поддержки"
				};
			}
		}

		public DataOperationResult SaveSampleCoordinates(int holderId, string row, string column, string barCode)
		{
			try
			{
				Sample curSample = GetSampleByBarCode(barCode);
				if (curSample == null)
				{
					return new DataOperationResult
					{
						ResultCode = DataOperationResult.ResultCodes.UnknownError,
						ErrorDescription = "Не найдены данные о пробе. Попробуйте еще раз или обратитесь в службу технической поддержки"
					};
				}

				curSample.HolderId = holderId;
				curSample.Row = row;
				curSample.Column = column;
				_postgresRepository.Update(curSample);

				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				string msg = $"Ошибка при попытке сохранить координаты пробы с barCode = {barCode} (SaveSampleCoordinates)";
				ServiceMethods.LogException(ex, msg, _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription =
						"Ошибка при попытке сохранить координаты пробы. Попробуйте еще раз или обратитесь в службу технической поддержки"
				};
			}
		}

		public DataOperationResult ArchiveHolder(int holderId)
		{
			try
			{
				var holders = _postgresRepository.Holders.Where(item => item.Id == holderId).ToList();
				if (holders.Any())
				{
					foreach (var holder in holders)
					{
						holder.Status = EHolderStatus.ARCHIVED;
						holder.ArchivedOn = DateTime.Now;
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
				string msg = $"Ошибка при попытке перевести штатив с id = {holderId} в архив (ArchiveHolder)";
				ServiceMethods.LogException(ex, msg, _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Ошибка при попытке перевести штатив в архив. Попробуйте еще раз или обратитесь в службу технической поддержки"
				};
			}
		}

		private Sample GetSampleByBarCode(string barCode)
		{
			Sample sample = null;

			var isLabelId = barCode.Length == 10 && barCode[0] == '5';   // if true barCode is sample_id, if false barCode is label_id
			if (isLabelId)
			{
				var samples = _postgresRepository.Samples.Where(item => item.LabelId == barCode).ToList();
				if (samples.Any())
					sample = samples[0];
			}
			else
			{
				int sampleMumber = int.Parse(barCode);
				var samples = _postgresRepository.Samples.Where(item => item.SampleNumber == sampleMumber).ToList();
				if (samples.Any())
					sample = samples[0];
			}

			return sample;
		}
	}
}
