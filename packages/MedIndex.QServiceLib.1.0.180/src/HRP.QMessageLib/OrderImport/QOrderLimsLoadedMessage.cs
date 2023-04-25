using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace HRP.QMessageLib
{
    [Serializable]
    public class QOrderLimsLoadedMessage : IQMessage
    {
        public object Order { get; set; }
    }
}
