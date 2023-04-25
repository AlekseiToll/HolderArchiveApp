using System;
using System.Collections.Generic;
using System.Linq;
using Helix.Logger;
using HolderArchiveApp.Service;

namespace HolderArchiveApp.Domain.Entities
{
	public class WorkflowRecord
	{
		private static readonly HelixLogger _logger = HelixLogManager.GetCurrentClassLogger();
		private static readonly Dictionary<int, string> _statusesDictionary = new Dictionary<int, string>
		{
			{0, "A"},
			{1, "X"},
			{2, "C"},
			{3, "U"},
			{4, "I"},
			{5, "P"}
		};

		public WorkflowRecord()
		{
			ContainerTypes = new List<ContainerTypeForWorkflow>();
		}
		public int Id { get; set; }
		public string Workflow { get; set; }

		public int? Statuses { get; set; }

		public virtual List<ContainerTypeForWorkflow> ContainerTypes { get; set; }

		public List<string> GetStatusesAsStrings()
		{
			int cntStatuses = Enum.GetNames(typeof (ESampleStatus)).Length;
			List<string> statuses = new List<string>();

			if (Statuses != null)
			{
				for (var i = 0; i < cntStatuses; i++)
				{
					if (((int) Statuses & (1 << i)) != 0)
					{
						statuses.Add(_statusesDictionary[i]);
					}
				}
			}
			return statuses;
		}

		public static int GetStatusesAsInt(IList<string> statuses)
		{
			int res = 0;
			if (statuses == null) return res;

			for (int i = 0; i < statuses.Count; i++)
			{
				try
				{
					int index = _statusesDictionary.First(x => x.Value == statuses[i]).Key;
					res |= (1 << index);
				}
				catch (InvalidOperationException ex)
				{
					ServiceMethods.LogException(ex, "InvalidOperationException in WorkflowRecordViewModel.GetStatusesAsInt:", _logger);
				}
			}
			return res;
		}
	}
}

