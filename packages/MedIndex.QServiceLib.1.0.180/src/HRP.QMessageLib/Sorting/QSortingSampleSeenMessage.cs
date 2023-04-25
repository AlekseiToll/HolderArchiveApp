using System;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QSortingSampleSeenMessage : IQMessage
    {
        public string SampleLabel { get; set; }
    }
}
