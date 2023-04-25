using System;
using System.Collections.Generic;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QSampleReadyMessage : IQMessage
    {
        public long OrderId { get; set; }
        public Guid OrderGuid { get; set; }
        public string OrderNumLw { get; set; }
        public string OrderCode { get; set; }

        public long SampleId { get; set; }
        public Guid SampleGuid { get; set; }
        public int SampleNumberLims { get; set; }
        public string SampleLabelId { get; set; }

        public int SampleStatus { get; set; }
        public DateTime DateChanged { get; set; }

        public List<string> Hxids { get; set; }
    }
}
