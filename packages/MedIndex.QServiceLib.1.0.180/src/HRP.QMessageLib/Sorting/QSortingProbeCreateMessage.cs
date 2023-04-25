using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QSortingProbeCreateMessage : IQMessage, IQMessageCreatedOn
    {
        public List<object> Probes { get; set; }
        public List<object> Tests { get; set; }
        public int ParentNumber { get; set; }
        public bool SorterWorkflow { get; set; }

        public DateTime CreatedOn { get; set; }
    }

    [Serializable]
    public class QSortingProbeCreateMessageV2 : IQMessage, IQMessageCreatedOn
    {
        public List<object> Probes { get; set; }
        public List<object> Tests { get; set; }
        public int ParentNumber { get; set; }
        public bool SorterWorkflow { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
