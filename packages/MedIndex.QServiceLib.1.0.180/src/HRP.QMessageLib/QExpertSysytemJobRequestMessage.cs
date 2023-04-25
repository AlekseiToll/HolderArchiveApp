using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QExpertSystemFilterRequestMessage : IQMessage, IQMessageCreatedOn
    {
        public DateTime CreatedOn { get; set; }
        public string FhirData { get; set; }
        public int JobId { get; set; }
        public string RequestType { get; set; }
    }

    [Serializable]
    public class QExpertSystemJobRequestMessage : IQMessage, IQMessageCreatedOn
    {
        public int JobId { get; set; }
        public string JsonData { get; set; }
        public string RequestType { get; set; }

        public DateTime CreatedOn { get; set; }
    }

    [Serializable]
    public class QExpertSystemDocumentCreateMessage : IQMessage, IQMessageCreatedOn
    {
        public int JobId { get; set; }
        public object ArtefactResult { get; set; }
        public string RequestType { get; set; }

        public DateTime CreatedOn { get; set; }
    }

    [Serializable]
    public class QExpertSystemInfoCreateMessage : IQMessage, IQMessageCreatedOn
    {
        public int JobId { get; set; }
        public object PathResult { get; set; }
        public object DefnitionResult { get; set; }
        public object ArtefactResult { get; set; }
        public string RequestType { get; set; }

        public DateTime CreatedOn { get; set; }
    }

    [Serializable]
    public class QExpertSystemFinishJobMessage : IQMessage, IQMessageCreatedOn
    {
        public int JobId { get; set; }
        public object ExpertSystemResult { get; set; }
        public string RequestType { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
