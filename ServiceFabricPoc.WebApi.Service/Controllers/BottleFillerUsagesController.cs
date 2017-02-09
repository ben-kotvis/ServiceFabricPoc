using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using ServiceFabricPoc.BottleFiller.Domain;
using System.Fabric;
using ServiceFabricPoc.WebApi.Service.Model;

namespace ServiceFabricPoc.WebApi.Service.Controllers
{

    public class BottleFillerUsagesController : ApiController
    {
        private const string BottleFillerServiceName = "BottleFillerActorService";

        [HttpPost]
        [Route("api/bottlefillerusages")]
        public async Task Post(AddUsage usage)
        {
            var activationContext = FabricRuntime.GetActivationContext();

            string applicationInstance = "ServiceFabricPoc.BottleFiller";

            var serviceUri = new Uri("fabric:/" + applicationInstance + "/" + BottleFillerServiceName);
            
            //We create a unique Guid that is associated with a customer order, as well as with the actor that represents that order's state.
            IBottleFillerActor bottleFiller = ActorProxy.Create<IBottleFillerActor>(new ActorId(Guid.Parse(usage.Id)), serviceUri);

            var message = new AddUsageMessage()
            {
                NumberOfGallons = usage.NumberOfGallons
            };

            await bottleFiller.AddUsage(message);

        }
    }
}
