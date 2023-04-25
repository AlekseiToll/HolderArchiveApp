using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helix.Logger;

namespace HolderArchiveApp.QueueProcessing.Service
{
	public static class LoggerHost
	{
		private static readonly HelixLogger _logger = new HelixLogger("Main");
		private static readonly HelixLogger _executionTimeLogger = new HelixLogger("ExecuteTime");

		public static HelixLogger LoggerInstance
		{
			get
			{
				return _logger;
			}
		}

		public static HelixLogger ExecuteTimeLogger
		{
			get
			{
				return _executionTimeLogger;
			}
		}
	}
}
