using System;
using System.Collections.Generic;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QOrderCompletedSaveTo1C : IQMessage
    {
        public string OrderCode1C { get; set; }
        public string OrderCode { get; set; }
        public Guid OrderGuid { get; set; }
        public int OrderStatus { get; set; }
        public List<object> ListTestStatus1CDtoList { get; set; }
    }
}
