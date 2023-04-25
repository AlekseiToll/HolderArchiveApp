using System;
using System.Collections.Generic;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QTestResultsSaveToHistoryMessage : IQMessage
    {
        public List<object> TestsToSampleItemsList { get; set; }
        public Guid OrderGuid { get; set; }

    }
}
