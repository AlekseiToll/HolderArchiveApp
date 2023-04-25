using System;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QSortingInnerProbeTextIdMessage : IQMessage
    {
        public string ParentLabelId { get; set; }
        public string HubCode { get; set; }
        public int ProbeSampleNumber { get; set; }
        public string ProbeTextId { get; set; }
        public int ParentTransportId { get; set; }
    }
}


