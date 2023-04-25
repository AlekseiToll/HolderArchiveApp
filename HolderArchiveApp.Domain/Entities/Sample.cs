using System;

namespace HolderArchiveApp.Domain.Entities
{
	public class Sample
	{
		// record_id пришлось добавить, т.к. EF не хочет сохранять первичный ключ
		// (Entity Framework configuration is set up for database-generated primary key columns)
		public int RecordId { get; set; }
		public int SampleNumber { get; set; }  // has 'unique' property in Database
		public string LabelId { get; set; }
		public string Status { get; set; }
		public string Workflow { get; set; }
		public string ContainerType { get; set; }
		public string SampleTemplate { get; set; }
		public DateTime ChangedOn { get; set; }
		public DateTime LoginOn { get; set; }
		public int? HolderId { get; set; }
		public string Row { get; set; }
		public string Column { get; set; }
	}
}
