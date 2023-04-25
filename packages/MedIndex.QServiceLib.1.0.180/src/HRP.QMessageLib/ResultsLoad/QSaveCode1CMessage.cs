using System;
using System.Collections.Generic;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QSaveCode1CMessage : IQMessage
    {
        public long? OrderID { get; set; }

        public Guid? OrderGuid { get; set; }

        public long? PacketID { get; set; }

        public string Code1C { get; set; }
    }
}
