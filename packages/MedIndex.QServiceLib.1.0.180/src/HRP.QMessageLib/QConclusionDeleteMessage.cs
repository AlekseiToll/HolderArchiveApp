using System;


namespace HRP.QMessageLib
{
    [Serializable]
    public class QConclusionDeleteMessage : IQMessage
    {
        public string OrderCode { get; set; }
        public Guid? OrderGuid { get; set; }
        public Guid? OrderItemGuid { get; set; }
        public int OrderItemID { get; set; }
        public DateTime DeletedOn { get; set; }
    }
}
