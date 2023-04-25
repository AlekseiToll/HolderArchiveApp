using System;
using System.Collections.Generic;

using RabbitMQ.Client;

namespace HRP.QServiceLib
{

    public class QWorker : QLog, IQWorker
    {
        protected string _hostUri = "";
        protected string _clientProvidedName = "";
        protected bool _sendJsonMessage = false;
        private IConnection _conn;
        private object _connLock = new object();

        protected QWorker() : base() { }

        protected IConnection GetConnection()
        {
            if (_conn == null || !_conn.IsOpen)
            {
                lock (_connLock)
                {
                    if (_conn == null || !_conn.IsOpen)
                    {
                        var factory = new ConnectionFactory() { Uri = _hostUri };
                        _conn = factory.CreateConnection(_clientProvidedName);
                    }
                }
            }

            return _conn;
        }

        private void CloseConnection()
        {
            if (_conn != null && _conn.IsOpen)
            {
                try
                {
                    _conn.Close(200, "Close connection by service client", 3000);
                }
                catch (Exception)
                {
                    _conn.Abort();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_isClosed) return;

            if (disposing)
            {
                CloseConnection();
            }
            base.Dispose(disposing);
        }

    }

}
