using System;
using System.Collections.Generic;
using System.Linq;
using DataAccessLevel.Abstract;
using HolderArchiveApp.Domain.Entities;
using HolderArchiveApp.Service;

namespace BusinessLogicLevel
{
	public class DataManagerStatuses : DataManagerBase
	{
		public DataManagerStatuses(IPostgresRepository postgresRepository, IAcgdRepository acgdRepository, IIdentityServicesRepository identityRepository):
			base(postgresRepository, acgdRepository, identityRepository)
		{
		}

		public DataOperationResult GetWorkflowRecordsList(out List<WorkflowRecord> listData)
		{
			try
			{
				listData = _postgresRepository.WorkflowRecords.OrderBy(i => i.Id).ToList();
				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				listData = null;
				ServiceMethods.LogException(ex, "Ошибка при попытке получить список записей рабочих потоков (GetWorkflowRecordsList):", _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Не удалось получить данные. Попробуйте еще раз или обратитесь в службу технической поддержки"
				};
			}
		}

		public DataOperationResult AddWorkflowRecord(WorkflowRecord workflowRecord)
		{
			if (workflowRecord.Statuses == 0 || workflowRecord.ContainerTypes.Count == 0 ||
					(workflowRecord.ContainerTypes.Count == 1 && workflowRecord.ContainerTypes[0].Name == ""))
			{
				string msg = "Не заполнены следующие обязательные поля:";
				if (workflowRecord.Statuses == 0) msg += " Статусы,";
				if (workflowRecord.ContainerTypes.Count == 0 ||
					(workflowRecord.ContainerTypes.Count == 1 && workflowRecord.ContainerTypes[0].Name == ""))
					msg += " Тип контейнера,";
				msg = msg.Remove(msg.Length - 1);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.ValidationError,
					ErrorDescription = msg
				};

			}

			using (var transaction = _postgresRepository.DbContext.Database.BeginTransaction())
			{
				try
				{
					List<WorkflowRecord> listWorkflowRecords;
					DataOperationResult res = GetWorkflowRecordsList(out listWorkflowRecords);
					if (res.ResultCode != DataOperationResult.ResultCodes.Ok)
					{
						return new DataOperationResult
						{
							ResultCode = DataOperationResult.ResultCodes.UnknownError,
							ErrorDescription = "Ошибка при добавлении записи рабочего потока. Попробуйте еще раз или обратитесь в службу технической поддержки"
						};
					}

					WorkflowRecord rec = listWorkflowRecords.Find(item => item.Workflow == workflowRecord.Workflow);
					if (rec != null) // already exists
					{
						DataOperationResult.ValidationErrorCodes validationResult = DataOperationResult.ValidationErrorCodes.None;
						validationResult |= DataOperationResult.ValidationErrorCodes.WorklowRecordAlreadyExists;
						return new DataOperationResult
						{
							ResultCode = DataOperationResult.ResultCodes.AlreadyExist,
							ValidationErrorCode = validationResult
						};
					}

					_postgresRepository.Add(workflowRecord);
					transaction.Commit();
					return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					string msg = "Ошибка при попытке добавить запись рабочего потока (AddWorkflowRecord). Объект: " +
								 ServiceMethods.ObjectToJson(workflowRecord);
					ServiceMethods.LogException(ex, msg, _logger);
					return new DataOperationResult
					{
						ResultCode = DataOperationResult.ResultCodes.UnknownError,
						ErrorDescription = "Ошибка при добавлении записи рабочего потока"
					};
				}
			}
		}

		public DataOperationResult UpdateWorkflowRecord(WorkflowRecord workflowRecord)
		{
			try
			{
				if (workflowRecord.Statuses == 0 || workflowRecord.ContainerTypes.Count == 0 ||
					(workflowRecord.ContainerTypes.Count == 1 && workflowRecord.ContainerTypes[0].Name == ""))
				{
					string msg = "Не заполнены следующие обязательные поля:";
					if (workflowRecord.Statuses == 0) msg += " Статусы,";
					if (workflowRecord.ContainerTypes.Count == 0 ||
						(workflowRecord.ContainerTypes.Count == 1 && workflowRecord.ContainerTypes[0].Name == ""))
						msg += " Тип контейнера,";
					msg = msg.Remove(msg.Length - 1);
					return new DataOperationResult
					{
						ResultCode = DataOperationResult.ResultCodes.ValidationError,
						ErrorDescription = msg
					};

				}

				_postgresRepository.Update(workflowRecord);
				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				string msg = "Ошибка при попытке обновить запись рабочего потока (UpdateWorkflowRecord). Объект: " + ServiceMethods.ObjectToJson(workflowRecord);
				ServiceMethods.LogException(ex, msg, _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Ошибка при попытке обновить запись рабочего потока"
				};
			}
		}

		public DataOperationResult GetWorkflows(out IQueryable<Workflow> listWorkflows)
		{
			listWorkflows = null;
			try
			{
				listWorkflows = _acgdRepository.Workflows;
				return new DataOperationResult { ResultCode = DataOperationResult.ResultCodes.Ok };
			}
			catch (Exception ex)
			{
				ServiceMethods.LogException(ex, "Ошибка при попытке получить список лабораторий (GetWorkflows):", _logger);
				return new DataOperationResult
				{
					ResultCode = DataOperationResult.ResultCodes.UnknownError,
					ErrorDescription = "Не удалось получить список лабораторий. Попробуйте еще раз или обратитесь в службу технической поддержки"
				};
			}
		}
	}
}
