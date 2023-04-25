using System;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QSampleSaveLwStatusMessage : IQMessage
    {
        public int SampleNumber { get; set; }
        public int Status { get; set; }
        public string Reviever { get; set; }
    }
}
