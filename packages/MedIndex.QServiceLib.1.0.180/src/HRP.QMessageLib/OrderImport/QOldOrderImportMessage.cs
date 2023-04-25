using System;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QOldOrderImportMessage : IQMessage
    {
        public object Order { get; set; }
        public object ExportPacket { get; set; }
    }
}
