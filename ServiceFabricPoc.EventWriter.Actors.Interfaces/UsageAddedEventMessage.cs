using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceFabricPoc.EventWriter.Actors.Domain
{
    [DataContract]
    public class UsageAddedEventMessage : EventMessage
    {
        [DataMember]
        public decimal NumberOfGallons { get; set; }
    }
}
