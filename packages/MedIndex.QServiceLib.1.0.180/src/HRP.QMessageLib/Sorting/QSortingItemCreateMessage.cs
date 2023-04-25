using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QSortingItemCreateMessage : IQMessage, IQMessageCreatedOn
    {
        public object Item { get; set; }
        public string ActionGuid { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
