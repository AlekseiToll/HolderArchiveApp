using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using DataAccessLevel.Abstract;
using Helix.Logger;
using HolderArchiveApp.Domain;
using HolderArchiveApp.Domain.Entities;
using HolderArchiveApp.Service;

namespace BusinessLogicLevel
{
    public abstract class DataManagerBase : IDisposable
    {
		protected IPostgresRepository _postgresRepository;
		protected IAcgdRepository _acgdRepository;
		protected IIdentityServicesRepository _identityRepository;
		protected static HelixLogger _logger = HelixLogManager.GetCurrentClassLogger();

	    protected DataManagerBase(IPostgresRepository postgresRepository, IAcgdRepository acgdRepository, IIdentityServicesRepository identityRepository)
		{
			_postgresRepository = postgresRepository;
			_acgdRepository = acgdRepository;
			_identityRepository = identityRepository;
		}

	    protected string GetUserLab(string userName)
	    {
			try
			{
				string curUserId = _identityRepository.GetUserByName(userName).Id;
				if (_postgresRepository.GetUserLabById(curUserId) != null)
					return _postgresRepository.GetUserLabById(curUserId).Laboratory;
				
				_logger.Error("Ошибка при попытке получить лабораторию пользователя (GetUserLab)");
				return string.Empty;
			}
			catch (Exception ex)
			{
				string msg = "Ошибка при попытке получить лабораторию пользователя (GetUserLab)";
				ServiceMethods.LogException(ex, msg, _logger);
				return string.Empty;
			}
		}

		public void Dispose()
	    {
		    _postgresRepository.Dispose();
		    _acgdRepository.Dispose();
			_identityRepository.Dispose();
	    }
    }
}
