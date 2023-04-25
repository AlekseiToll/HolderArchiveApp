using System;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QSortingSortReadyMessage : IQMessage
    {
        public string LabelId { get; set; }
        public string SampleGuid { get; set; }
        public string HubCode { get; set; }
    }
}
