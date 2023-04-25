using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLevel
{
	public class DataOperationResult
	{
		public enum ResultCodes
		{
			[Description("Ok")]
			Ok,
			[Description("Ошибка валидации")]
			ValidationError,
			[Description("Данные уже существуют")]
			AlreadyExist,
			[Description("Ошибка")]
			UnknownError
		}

		[Flags]
		public enum ValidationErrorCodes
		{
			None = 0,
			WorklowRecordAlreadyExists = 0x1
		}

		public ResultCodes ResultCode { get; set; }
		public ValidationErrorCodes ValidationErrorCode { get; set; }

		public string ErrorDescription { get; set; }

		public string MessageForUI
		{
			get
			{
				GetValidationErrors(ValidationErrorCode);
				return ErrorDescription;
			}
		}

		private void GetValidationErrors(ValidationErrorCodes validationErrors)
		{
			if (validationErrors.HasFlag(ValidationErrorCodes.WorklowRecordAlreadyExists))
			{
				ErrorDescription = "Запись для данного потока уже существует. Отредактируйте существующую запись.";
			}
		}
	}
}
