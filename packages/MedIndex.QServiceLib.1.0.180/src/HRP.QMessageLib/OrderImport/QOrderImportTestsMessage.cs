using System;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QOrderImportTestsMessage : IQMessage
    {
        public long OrderId { get; set; }
        public Guid PacketGuid { get; set; }
    }
}
