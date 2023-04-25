using System;

using RabbitMQ.Client;

namespace HRP.QServiceLib
{
    public interface IQWorker : IDisposable
    {
        bool IsClosed { get; }
    }
}
