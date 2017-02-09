using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceFabricPoc.WebApi.Service.Model
{
    public class AddUsage
    {
        public string Id { get; set; }
        public decimal NumberOfGallons { get; set; }
    }
}
