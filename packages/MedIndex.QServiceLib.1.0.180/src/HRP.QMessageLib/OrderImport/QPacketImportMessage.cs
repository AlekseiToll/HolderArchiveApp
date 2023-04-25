using System;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QPacketImportMessage : IQMessage
    {
        public object Packet { get; set; }
    }
}
