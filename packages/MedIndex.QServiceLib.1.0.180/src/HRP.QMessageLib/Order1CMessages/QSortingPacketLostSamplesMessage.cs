using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QSortingPacketLostSamplesMessage : IQMessage
    {
        public string OrderCode { get; set; }
        public string CustomerID { get; set; }
        public string AccountID { get; set; }
        public Guid OrderGuid { get; set; }
        public DateTime OrderDateCreate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }

        public int SampleNumberLIS { get; set; }
        public string SampleLabelID { get; set; }
        public Guid SampleGuid { get; set; }

        public Dictionary<Guid, string> TestsInfo { get; set; }
    }
}
