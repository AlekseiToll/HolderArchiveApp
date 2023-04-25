using System;


namespace HRP.QMessageLib
{
    [Serializable]
    public class QConclusionCreateMessage : IQMessage, IQMessageCreatedOn
    {
        public string OrderCode { get; set; }
        public Guid? OrderGuid { get; set; }
        public Guid? OrderItemGuid { get; set; }
        public int OrderItemID { get; set; }
        public string HXID { get; set; }
        public string ContractCode { get; set; }
        public string AccountNumber { get; set; }
        public string Department { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public DateTime OrderCreatedOn { get; set; }
        public int ConclusionID { get; set; }   //ID in LW
        public DateTime CreatedOn { get; set; }

    }
}
