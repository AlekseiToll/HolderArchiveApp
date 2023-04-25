using System;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QTestSaveLwStatusMessage : IQMessage
    {
        public long TestID { get; set; }
        public int TestNumber { get; set; }
        public int Status { get; set; }
        public string Reviever { get; set; }
    }
}
