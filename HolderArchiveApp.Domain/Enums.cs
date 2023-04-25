using System;

namespace HolderArchiveApp.Domain
{
	public enum EHolderStatus
	{
		NEW,            // = virtual
		EMPTY,
		IN_WORK,
		ARCHIVED
	}

	public enum ESampleTemplate
	{
		SMP_IN
	}

	[Flags]
	public enum ESampleStatus
	{
		A = 1,      // авторизованные
		X = 2,      // отмененные
		C = 4,      // законченные
		U = 8,      // не принятые
		I = 16,     // не начатые
		P = 32      // в процессе
	}

	public enum EUserRole
	{
		Archive,            // доступна страница "Архивация"
		ArchiveDropper,     // доступна страница "Сброс"
		ArchiveAdmin        // доступен полный функционал модуля
	}
}