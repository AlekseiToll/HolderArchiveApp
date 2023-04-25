using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QQCSortingResultsRecieveMessage : IQMessage
    {
        public List<QQCSortingResultsRecieveResultItem> ResultItems { get; set; }
    }

    [Serializable]
    public class QQCSortingResultsRecieveResultItem
    {
        public string SampleNumber { get; set; }
        public string TestAlias { get; set; }
        public string StringResult { get; set; }
        public double NumericValue { get; set; }
        public string InstrumentId { get; set; }
        public string Place { get; set; }
    }
}
