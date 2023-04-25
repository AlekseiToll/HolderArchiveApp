using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QQCStatusMessage : IQMessage
    {
        public object Status { get; set; }
    }

}
