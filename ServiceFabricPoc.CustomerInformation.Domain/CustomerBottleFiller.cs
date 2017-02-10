using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceFabricPoc.CustomerInformation.Domain
{
    [DataContract]
    public class CustomerBottleFiller
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string LocationDescription { get; set; }
        [DataMember]
        public string SerialNumber { get; set; }
        [DataMember]
        public string  Type { get; set; }
        [DataMember]
        public string  ProductFamily { get; set; }
    }
}
