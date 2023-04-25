using System;
using System.Diagnostics;

namespace HRP.QServiceLib
{
    public class QLog : IDisposable
    {
        protected EventLog _eventLog;
        protected bool _isClosed;

        public bool IsClosed { get { return _isClosed; } }

        //public EventLog EventLog { get { return _eventLog; } }

        protected QLog()
        {
            _eventLog = LogInit();
        }

        public static EventLog LogInit()
        {
            string sourceName = "QRabbit";
            string logName = "QRabbitService";

            // EventLog.Delete("QService");
            // EventLog.DeleteEventSource(sourceName);

            if (!EventLog.SourceExists(sourceName))
            {
                EventLog.CreateEventSource(sourceName, logName);
            }

            EventLog eventLog = new System.Diagnostics.EventLog();

            eventLog.Source = sourceName;
            eventLog.Log = logName;

            return eventLog;
        }

        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_eventLog != null)
                {
                    _eventLog.Close();
                    _eventLog.Dispose();
                }
            }
            _isClosed = true;
        }
    }

}
