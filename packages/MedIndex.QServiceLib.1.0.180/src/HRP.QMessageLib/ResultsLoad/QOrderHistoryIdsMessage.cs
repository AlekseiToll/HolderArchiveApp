using System;

namespace HRP.QMessageLib
{
    //Depricated - pavlikov 20161021

    [Serializable]
    public class QOrderHistoryIdsMessage : IQMessage
    {
        public Guid OrderGuid { get; set; }
        public string LwNumber { get; set; }
        public string Code1C { get; set; }
    }
}
