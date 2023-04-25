using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QSampleLimsStatusMessage : IQMessage, IQMessageCreatedOn
    {
        public int SampleNumberLims { get; set; }
        public Guid SampleGuid { get; set; }
        public string SampleStatus { get; set; }
        public string OrderNumLw { get; set; }
        public DateTime ChangedOn { get; set; }
        public DateTime? ReviewedOn { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
