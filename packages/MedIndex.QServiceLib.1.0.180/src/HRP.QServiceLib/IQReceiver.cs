using System;


namespace HRP.QServiceLib
{
    public interface IQReceiver : IDisposable
    {
        bool IsClosed { get; }
        bool PauseReceive { get; set; }
        void Receive(int p = 1);
        void StopReceive();
    }
}
