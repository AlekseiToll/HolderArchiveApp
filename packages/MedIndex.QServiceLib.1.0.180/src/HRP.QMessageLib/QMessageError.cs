using System;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QMessageError : IQMessage
    {
        public IQMessage QMessage { get; set; }
        public Type MessageType { get; set; }
        public string BackExchange { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorJson { get; set; }
        public string MessageJson { get; set; }
        public Exception Error { get; set; }
        public DateTime StartedOn { get; set; }
        public DateTime LoggedOn { get; set; }
    }
}
