using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceFabricPoc.EventWriter.Actors.Domain
{
    [DataContract]
    [KnownType(typeof(UsageAddedEventMessage))]
    public class EventMessage
    {
        [DataMember]
        public string ActorId { get; set; }

        [DataMember]
        public DateTimeOffset EventTime { get; set; }
    }
}
