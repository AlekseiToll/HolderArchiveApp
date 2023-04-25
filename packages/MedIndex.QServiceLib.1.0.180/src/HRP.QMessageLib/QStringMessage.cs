using System;

using HRP.QMessageLib;


namespace HRP.TestQService
{
    [Serializable]
    public class QStringMessage : IQMessage
    {
        public string Body { get; set; }
    }
}
