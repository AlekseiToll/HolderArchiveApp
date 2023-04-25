using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QOrderSendSampleResultFactMessage : IQMessage
    {
        public object Order { get; set; }
        public List<string> Emails { get; set; }
    }
}
