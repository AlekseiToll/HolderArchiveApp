using System;
using System.Collections.Generic;

using HRP.QMessageLib;

namespace HRP.QServiceLib
{
    public interface IQPublisher : IDisposable
    {
        bool IsClosed { get; }
        IQMessage Publish(IQMessage message);
        List<IQMessage> Publish(List<IQMessage> messages, int p = 1);
    }
}
