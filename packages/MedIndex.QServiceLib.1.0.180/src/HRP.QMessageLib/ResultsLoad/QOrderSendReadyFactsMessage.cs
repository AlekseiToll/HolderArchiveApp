using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QOrderSendReadyFactsMessage : IQMessage
    {
        public Guid OrderGuid { get; set; }
        public string OrderCode { get; set; }
        public string OrderLwCode { get; set; }
        public int OrderStatus { get; set; }
    }
}
