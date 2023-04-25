using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace HRP.QMessageLib
{
    [Serializable]
    public class QOrderLimsStatusMessage : IQMessage, IQMessageCreatedOn
    {
        public long OrderId { get; set; } // deprecated
        public string OrderNumLw { get; set; }
        public string LimsStatus { get; set; }
        public DateTime ChangedOn { get; set; }
        public bool HasNoSamples { get; set; } //deprecated

        public DateTime CreatedOn { get; set; }

        //public Guid OrderGuid { get; set; } //pavlikov - for future
    }
}
