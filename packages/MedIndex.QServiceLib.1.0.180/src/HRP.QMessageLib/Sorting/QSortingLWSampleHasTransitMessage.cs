using System;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QSortingLWSampleHasTransitMessage : IQMessage
    {
        public int SampleNumber { get; set; }
        public string HubCode { get; set; }
    }
}
