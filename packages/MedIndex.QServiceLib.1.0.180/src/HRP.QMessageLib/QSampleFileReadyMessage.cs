using System;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QSampleFileReadyMessage : IQMessage
    {
        public DateTime DateOfCreateFromTriton { get; set; }
        public DateTime DateOfReceiveFromQueue { get; set; }
        public int FileSize { get; set; }
        public bool IsCache { get; set; }
        public int SampleNumber { get; set; }
        public string ServerName { get; set; }
        public int TimeOfGenerateFromTriton { get; set; }
        public string FileName { get; set; }
    }
}