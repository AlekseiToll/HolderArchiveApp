using System;
using System.Collections.Generic;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QOrderTestsUpdateFromLimsMessage : IQMessage
    {
        public long OrderId { get; set; }
        public string OrderNumLw { get; set; }
        public int OrderStatus { get; set; }
        public List<int> TestNumbers { get; set; }
        public List<object> UpdatedTests { get; set; }
        public List<object> Results { get; set; }
        public List<long> DeletedTestIds { get; set; }
        public Dictionary<int, int> SampleProbeNumbers { get; set; }
        public Dictionary<int, string> SampleStatuses { get; set; }
        public Dictionary<int, DateTime> SampleChangedOn { get; set; }
        public Dictionary<int, DateTime?> SampleReviewedOn { get; set; }
    }
}
