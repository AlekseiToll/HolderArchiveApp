using System;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QOrderImportMessage : IQMessage
    {
        public object Order { get; set; }
    }
}
