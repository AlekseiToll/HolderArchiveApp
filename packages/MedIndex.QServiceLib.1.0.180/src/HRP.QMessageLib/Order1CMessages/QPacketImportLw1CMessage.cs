using System;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QPacketImportLw1CMessage : IQMessage
    {
        public object Packet { get; set; }
    }
}
