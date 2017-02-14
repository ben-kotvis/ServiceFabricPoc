using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceFabricPoc.WebApi.Service.Model
{
    public class BottleFillerStatistics
    {
        public string Id { get; set; }

        public string SerialNumber { get; set; }

        public decimal TotalGallons { get; set; }

        public decimal CurrentFilterGallonsUsed { get; set; }

        public DateTimeOffset FilterInstallDate { get; set; }

        public DateTimeOffset ServiceDate { get; set; }

        public string OnBoardUser { get; set; }
    }
}
