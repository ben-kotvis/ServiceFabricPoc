using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceFabricPoc.BottleFiller.Domain
{
    [DataContract]
    public class BottleFillerStatistics
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string SerialNumber { get; set; }

        [DataMember]
        public decimal TotalGallons { get; set; }

        [DataMember]
        public decimal CurrentFilterGallonsUsed { get; set; }

        [DataMember]
        public DateTimeOffset FilterInstallDate { get; set; }

        [DataMember]
        public DateTimeOffset ServiceDate { get; set; }

        [DataMember]
        public string OnBoardUser { get; set; }
    }
}
