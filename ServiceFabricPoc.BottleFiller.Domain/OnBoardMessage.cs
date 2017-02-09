using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceFabricPoc.BottleFiller.Domain
{
    [DataContract]
    public class OnBoardMessage
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string SerialNumber { get; set; }

        [DataMember]
        public string OnBoardUser { get; set; }

    }
}
