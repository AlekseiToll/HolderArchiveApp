using System;
using System.Collections.Generic;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QTestStatusCheckInLimsMessage : IQMessage
    {
        public long OrderId { get; set; }
        public string OrderNumLw { get; set; }
        public int OrderStatus{ get; set; }
        public List<object> Tests { get; set; }
    }
}
