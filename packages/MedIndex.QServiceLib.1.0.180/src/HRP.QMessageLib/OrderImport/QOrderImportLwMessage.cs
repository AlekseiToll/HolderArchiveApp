using System;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QOrderImportLwMessage : IQMessage
    {
        public object Order { get; set; }
    }
}
