using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace HolderArchiveApp.QueueProcessing.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
			if (Environment.UserInteractive)
            {
                var serviceToRun = new HolderArchiveAppProcessingService();
                serviceToRun.TestStartupAndStop(args);
            }
            else
            {
	            var servicesToRun = new ServiceBase[] { new HolderArchiveAppProcessingService() };
	            ServiceBase.Run(servicesToRun);
            }
        }
    }
}
