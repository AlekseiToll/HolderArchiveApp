using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRP.QMessageLib
{
    [Serializable]
    public class QSortingProbeSave1CDocument : IQMessage, IQMessageCreatedOn
    {
        public int SampleId { get; set; }
        public String DocCode1C { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
