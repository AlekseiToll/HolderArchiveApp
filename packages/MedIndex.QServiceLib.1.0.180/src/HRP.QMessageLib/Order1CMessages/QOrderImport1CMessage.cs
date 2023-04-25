using System;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QOrderImport1CMessage : IQMessage
    {
        public object Order { get; set; }
    }
}
