using System;
using System.Collections.Generic;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QQualityControlResultsRecieveMessage : IQMessage
    {
        public List<QSortingResultsRecieveResultItem> ResultItems { get; set; }
    }
}
