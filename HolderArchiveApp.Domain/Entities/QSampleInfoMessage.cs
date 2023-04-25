using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRP.QMessageLib;

namespace HolderArchiveApp.Domain.Entities
{
	[Serializable]
	public class QSampleInfoMessage : IQMessage
	{
		public int SampleNumber { get; set; }
		public string SampleLabelId { get; set; }
		public string ContainerType { get; set; }
		public string Workflow { get; set; }
		public string SampleStatus { get; set; }
		public string SampleTemplate { get; set; }
		public DateTime ChangedOn { get; set; }
		public DateTime LoginOn { get; set; }
	}
}
