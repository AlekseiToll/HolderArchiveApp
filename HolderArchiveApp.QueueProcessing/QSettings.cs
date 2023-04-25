using System;
using System.IO;
using System.Xml.Serialization;

namespace HolderArchiveApp.QueueProcessing.Service
{
	internal struct QSettings
	{
		public string HostUri { get; set; }
		public int ParallelIndex { get; set; }
		public string ExPrefix { get; set; }
		public int RetiresTimeout { get; set; }
		public int RetriesCount { get; set; }
		public string ClientProvidedName { get; set; }
		public bool SendJsonMessage { get; set; }

		public string WebServerUrl { get; set; }
		//public string WebServerIp { get; set; }
		//public int WebServerPort { get; set; }
		//public string WebServerUser { get; set; }
		//public string WebServerPswd { get; set; }
	}
}
