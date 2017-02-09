using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceFabricPoc.BottleFiller.Domain
{
    [DataContract]
    public class AddUsageMessage
    {
        [DataMember]
        public decimal NumberOfGallons { get; set; }
    }
}
