using System;
using System.Collections.Generic;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QSamplePrintInfoMessage : IQMessage
    {
        public long OrderId { get; set; }
        public Guid OrderGuid { get; set; }
        public string OrderNumLw { get; set; }
        public string OrderCode { get; set; }
        public DateTime OrderDateCreate { get; set; }
        public string AccountID { get; set; }
        public string ContractID { get; set; }
        public string Department { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public bool HasPositiveHiv { get; set; }

        public long SampleId { get; set; }
        public Guid SampleGuid { get; set; }
        public int SampleNumberLims { get; set; }
        public string SampleLabelId { get; set; }

        public int SampleStatus { get; set; }
        public DateTime DateChanged { get; set; }
        public DateTime? DateReviewed { get; set; }
        public DateTime DateLogged { get; set; }

        public List<string> Hxids { get; set; }
    }
}



