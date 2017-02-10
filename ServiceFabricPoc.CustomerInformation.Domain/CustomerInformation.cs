using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceFabricPoc.CustomerInformation.Domain
{
    [DataContract]
    public class CustomerInformation
    {
        [DataMember]
        public List<CustomerBottleFiller> BottleFillers { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string MainLocationId { get; set; }
    }
}
