using System;
using System.Collections.Generic;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QSampleLimsTestResultsMessage : IQMessage, IQMessageCreatedOn
    {
        public int SampleNumberLims { get; set; }
        public string SampleStatus { get; set; }
        public string OrderNumLw { get; set; }

        public List<object> Tests { get; set; }
        public List<object> Results { get; set; }

        public DateTime CreatedOn { get; set; }


        //public Guid SampleGuid { get; set; }   //pavlikov - for future
        //public Guid OrderGuid { get; set; }   //pavlikov - for future

    }
}
