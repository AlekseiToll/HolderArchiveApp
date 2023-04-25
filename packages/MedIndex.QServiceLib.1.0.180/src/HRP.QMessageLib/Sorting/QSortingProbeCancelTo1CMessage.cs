using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QSortingProbeCancelTo1CMessage : IQMessage, IQMessageCreatedOn
    {
        public String HubCode { get; set; }
        public String LabelCode { get; set; }
        public String Cancel { get; set; }
        public String Code1C { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
