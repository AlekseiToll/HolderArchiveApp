using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using HolderArchiveApp.Domain;
using HolderArchiveApp.Domain.Entities;

namespace HolderArchiveApp.Models
{
	public class HolderTypeViewModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public Color HolderColor { get; set; }

		public string ColorAsString
		{
			get { return ColorTranslator.ToHtml(HolderColor); }
			set { HolderColor = ColorTranslator.FromHtml(value); }
		}

		public IList<string> ContainerTypes { get; set; }
		//public IList<Holder> Holders { get; set; }

		public int CountCellsHorizontal { get; set; } // 1 ... 100
		public int CountCellsVertical { get; set; } // 1 ... 100
		public int TimeLimit { get; set; } // 1 ... 9125
		public string LaboratoryName { get; set; }
		public string ChangedOn { get; set; }
		public string CreatedOn { get; set; }

		public HolderTypeViewModel()
		{
		}

		public HolderTypeViewModel(HolderType dataModel)
		{
			Id = dataModel.Id;
			Name = dataModel.Name;
			HolderColor = dataModel.HolderColor;
			CountCellsHorizontal = dataModel.CountCellsHorizontal;
			CountCellsVertical = dataModel.CountCellsVertical;
			TimeLimit = dataModel.TimeLimit;
			LaboratoryName = dataModel.LaboratoryName;
			ChangedOn = dataModel.ChangedOn.ToString("dd-MM-yyyy HH:mm:ss");
			CreatedOn = dataModel.CreatedOn.ToString("dd-MM-yyyy HH:mm:ss");

			ContainerTypes = new List<string>();
			foreach (var containerType in dataModel.ContainerTypes)
			{
				ContainerTypes.Add(containerType.Name);
			}

			int countNew = dataModel.Holders.Count(h => h.Status == EHolderStatus.NEW);
			int countNotNew = dataModel.Holders.Count - countNew;
			CountNewAndOtherHolders = $"{countNew}/{countNotNew}";
		}

		public HolderType GetHolderType()
		{
			HolderType holderType = new HolderType
			{
				Id = Id,
				Name = Name,
				HolderColor = HolderColor,
				CountCellsHorizontal = CountCellsHorizontal,
				CountCellsVertical = CountCellsVertical,
				TimeLimit = TimeLimit,
				LaboratoryName = LaboratoryName,
				ChangedOn = DateTime.ParseExact(ChangedOn, "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture,
System.Globalization.DateTimeStyles.AllowWhiteSpaces),
				CreatedOn = DateTime.ParseExact(CreatedOn, "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture,
System.Globalization.DateTimeStyles.AllowWhiteSpaces),
				ContainerTypes = new List<ContainerTypeForHolderType>()
			};
			if (ContainerTypes != null)
			{
				foreach (var containerTypeName in ContainerTypes)
				{
					holderType.ContainerTypes.Add(new ContainerTypeForHolderType { Name = containerTypeName, HolderType = holderType });
				}
			}
			return holderType;
		}

		public string CountNewAndOtherHolders { get; private set; }
	}
}