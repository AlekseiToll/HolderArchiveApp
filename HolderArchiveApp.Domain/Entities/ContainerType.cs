using System;

namespace HolderArchiveApp.Domain.Entities
{
	/// <summary>
	/// This class is used for operations with Container types in PostgreSQL
	/// </summary>
	public class ContainerTypeForHolderType
	{
		public int RecordId { get; set; }
		public string Name { get; set; }
		public virtual HolderType HolderType { get; set; }
	}

	/// <summary>
	/// This class is used for operations with Container types in PostgreSQL
	/// </summary>
	public class ContainerTypeForWorkflow
	{
		public int RecordId { get; set; }
		public string Name { get; set; }
		public virtual WorkflowRecord WorkflowRecord { get; set; }
	}

	/// <summary>
	/// This class is used for getting data from ACGD
	/// </summary>
	public class ContainerTypeACGD
	{
		//public int Id { get; set; }
		public string Name { get; set; }
		public string ChangedBy { get; set; }
		public DateTime? ChangedOn { get; set; }
		//public char Removed { get; set; } 
		public string Description { get; set; }
		public string GroupName { get; set; }
		public bool IsRemoved { get; set; }
		public DateTime TimeStamp { get; set; }

		public string NameAndDescription 
		{
			get { return $"{Name}, {Description}"; }
		}
	}
}
