using System;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QOldPacketImportMessage : IQMessage
    {
        public object Packet { get; set; }
    }
}
