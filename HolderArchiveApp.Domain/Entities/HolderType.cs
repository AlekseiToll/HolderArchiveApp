using System;
using System.Collections.Generic;
using System.Drawing;

namespace HolderArchiveApp.Domain.Entities
{
	public class HolderType
	{
		public HolderType()
		{
			ContainerTypes = new List<ContainerTypeForHolderType>();
			//Holders = new List<Holder>();
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public Color HolderColor { get; set; }

		public int HolderColorArgb
		{
			get { return HolderColor.ToArgb(); }
			set { HolderColor = Color.FromArgb(value); }
		}

		public virtual IList<ContainerTypeForHolderType> ContainerTypes { get; set; }

		public virtual IList<Holder> Holders { get; set; }

		public int CountCellsHorizontal { get; set; } // 1 ... 100
		public int CountCellsVertical { get; set; } // 1 ... 100
		public int TimeLimit { get; set; } // 1 ... 9125  срок
		public string LaboratoryName { get; set; }
		public DateTime ChangedOn { get; set; }
		public DateTime CreatedOn { get; set; }
	}

	public class HolderTypeToArchive
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int NextHolderToArchive { get; set; }
		public string Color { get; set; }
		public string Info { get; set; }
	}
}
