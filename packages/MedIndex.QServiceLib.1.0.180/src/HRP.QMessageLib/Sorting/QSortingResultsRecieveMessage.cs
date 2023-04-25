using System;
using System.Collections.Generic;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QSortingResultsRecieveMessage : IQMessage
    {
        public List<QSortingResultsRecieveResultItem> ResultItems { get; set; }
    }

    [Serializable]
    public class QSortingResultsRecieveResultItem
    {
        public string SampleNumber { get; set; }
        public string TestAlias { get; set; }
        public string StringResult { get; set; }
        public double NumericValue { get; set; }
        public string InstrumentId { get; set; }
    }
}
