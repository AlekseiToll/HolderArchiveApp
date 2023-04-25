using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace HRP.QMessageLib
{
    [Serializable]
    public class QOrderItemReadyCheckMessage : IQMessage
    {
        public long SampleID { get; set; }
        public int SampleNumber { get; set; }

        public long OrderID { get; set; }
        public string OrderNum { get; set; }
    }
}
