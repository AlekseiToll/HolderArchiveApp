using System;

namespace HolderArchiveApp.Domain.Entities
{
	public class Laboratory
	{
		public string Code { get; set; }
		public string Description { get; set; }
		public bool IsRemoved { get; set; }
		public DateTime TimeStamp { get; set; }
	}
}
