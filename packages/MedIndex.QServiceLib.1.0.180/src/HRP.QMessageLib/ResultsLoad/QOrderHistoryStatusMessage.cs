using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace HRP.QMessageLib
{
    [Serializable]
    public class QOrderHistoryStatusMessage : IQMessage
    {
        public Guid OrderGuid { get; set; }
        public int OrderStatus { get; set; }
        public DateTime ChangedOn { get; set; }
    }
}
