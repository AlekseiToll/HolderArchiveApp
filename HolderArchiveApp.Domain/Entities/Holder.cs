using System;

namespace HolderArchiveApp.Domain.Entities
{
	public class Holder
	{
		public int Id { get; set; }

		public int HolderTypeId { get; set; }

		public EHolderStatus Status { get; set; } 
		public DateTime CreatedOn { get; set; }
		public DateTime? DeletedOn { get; set; }
		public DateTime? ArchivedOn { get; set; }

		public string StatusAsString
		{
			get
			{
				switch (Status)
				{
					case EHolderStatus.NEW: return "Новый";
					case EHolderStatus.EMPTY: return "Пустой";
					case EHolderStatus.IN_WORK: return "В работе";
					case EHolderStatus.ARCHIVED: return "В архиве";
					default: return "Неизвестно";
				}
			}
		}

		public HolderType HolderType { get; set; }
	}

	public class HolderToReset
	{
		public string HolderTypeName { get; set; }
		public int Id { get; set; }
		public DateTime ArchivedOn { get; set; }
		public int TimeLimit { get; set; } // 1 ... 9125  срок
	}
}
