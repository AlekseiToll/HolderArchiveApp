using System;
using System.Configuration;
using HRP.QServiceLib;

namespace HolderArchiveApp.QueueProcessing.Service
{
    internal class Shell : ShelfService
    {
	    private QListenerService _service;
	    private QSettings _settings;

        public new void Start()
        {
			_settings = GetSettings();
			_service = new QListenerService(_settings);
			_service.Start();
		}

        public new void Stop()
        {
            _service.Stop();
        }

		internal QSettings GetSettings()
		{
			try
			{
				return new QSettings()
				{
					HostUri = GetSetting<string>("QueueUri"),
					ParallelIndex = GetSetting<int>("ParallelIndex"),
					ExPrefix = GetSetting<string>("ExchangePrefix"),
					RetriesCount = GetSetting<int>("QueueRetries"),
					RetiresTimeout = GetSetting<int>("QueueReprocessingTimeout"),
					ClientProvidedName = GetSetting<string>("ClientProvidedName"),
					SendJsonMessage = GetSetting<bool>("SendJsonMessage"),

					WebServerUrl = GetSetting<string>("WebServerUrl")
					//WebServerIp = GetSetting<string>("WebServerIp"),
					//WebServerPort = GetSetting<int>("WebServerPort"),
					//WebServerUser = GetSetting<string>("WebServerUser"),
					//WebServerPswd = GetSetting<string>("WebServerPswd")
				};
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Не удалось получить настройки!", ex);
			}
		}

		public T GetSetting<T>(string settingName)
		{
			try
			{
				var value = ConfigurationManager.AppSettings[settingName];
				return (T)Convert.ChangeType(value, typeof(T));
			}
			catch (Exception ex)
			{
				throw new ArgumentException("Не удалось прочитать настройку " + settingName, ex);
			}
		}
	}
}
