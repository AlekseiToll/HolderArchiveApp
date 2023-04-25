using System;
using System.Collections.Generic;
using System.Linq;
using Helix.Logger;
using HolderArchiveApp.Domain;
using HolderArchiveApp.Domain.Entities;
using HolderArchiveApp.Service;

namespace HolderArchiveApp.Models
{
	public class WorkflowRecordViewModel
	{
		private static readonly HelixLogger _logger = HelixLogManager.GetCurrentClassLogger();

		public int Id { get; set; }
		public string Workflow { get; set; }
		public IList<string> Statuses { get; set; }
		public IList<string> ContainerTypes { get; set; }
		public string StatusesAsString { get; set; }

		public WorkflowRecordViewModel()
		{
		}

		public WorkflowRecordViewModel(WorkflowRecord dataModel)
		{
			Id = dataModel.Id;
			Workflow = dataModel.Workflow;
			ContainerTypes = new List<string>();
			foreach (var containerType in dataModel.ContainerTypes)
			{
				ContainerTypes.Add(containerType.Name);
			}

			Statuses = dataModel.GetStatusesAsStrings();
			if (Statuses.Count > 0)
			{
				foreach (var s in Statuses)
					StatusesAsString += (s + ", ");
				if (StatusesAsString.Length > 2)
					StatusesAsString.Remove(StatusesAsString.Length - 2);
			}
		}

		public WorkflowRecord GetWorkflowRecord()
		{
			WorkflowRecord record = new WorkflowRecord
			{
				Id = Id,
				Workflow = Workflow,
				ContainerTypes = new List<ContainerTypeForWorkflow>(),
				Statuses = WorkflowRecord.GetStatusesAsInt(Statuses)
			};
			if (ContainerTypes != null)
			{
				foreach (var containerTypeName in ContainerTypes)
				{
					record.ContainerTypes.Add(new ContainerTypeForWorkflow { Name = containerTypeName, WorkflowRecord = record });
				}
			}
			return record;
		}
	}
}