using System;
using System.Diagnostics;
using System.ServiceProcess;
using HRP.QServiceLib;

namespace HolderArchiveApp.QueueProcessing.Service
{
    public partial class HolderArchiveAppProcessingService : ServiceBase
    {
        private readonly EventLog _eventLog;
        private readonly string _serviceName;
        private readonly Shell _shell;

        public HolderArchiveAppProcessingService()
        {
            InitializeComponent();

            _serviceName = this.GetType().Name;
            _eventLog = QLog.LogInit();
            _shell = new Shell();   
        }

        protected override void OnStart(string[] args)
        {
            _shell.Start();
            _eventLog.WriteEntry($"{_serviceName} OnStart", EventLogEntryType.SuccessAudit);
        }

        protected override void OnStop()
        {
            _shell.Stop();
            _eventLog.WriteEntry($"{_serviceName} OnStop", EventLogEntryType.SuccessAudit);
        }

        internal void TestStartupAndStop(string[] args)
        {
            this.OnStart(args);
            Console.WriteLine("HolderArchiveAppQueueProcessingService started.");
            Console.WriteLine("\r\nPress Enter to close programm.");
            Console.ReadLine();
            this.OnStop();
        }

        public new void Dispose()
        {
            if (_eventLog != null)
            {
                _eventLog.Close();
                _eventLog.Dispose();
            }

            base.Dispose();
        }
    }
}
