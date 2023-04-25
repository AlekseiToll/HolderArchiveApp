using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QSortingProbeCreateJobMessage : IQMessage, IQMessageCreatedOn
    {
        public string SampleNumber { get; set; }
        public string SampleType { get; set; }
        public string ParentLabelId { get; set; }
        public string HubCode { get; set; }
        public string DestHubCode { get; set; }
        public string Workflow { get; set; }
        public string TransportCondition { get; set; }
        public string OrderCode { get; set; }
        public string ParentTransportCode { get; set; }

        public string OperationGuid { get; set; }

        public List<object> Tests { get; set; }

        public DateTime CreatedOn { get; set; }

    }
}
