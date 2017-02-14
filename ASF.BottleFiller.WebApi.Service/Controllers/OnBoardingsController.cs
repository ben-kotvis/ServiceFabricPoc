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

    public class OnBoardingsController: ApiController
    {
        private const string BottleFillerServiceName = "BottleFillerActorService";

        [HttpPost]
        [Route("api/onboardings")]
        public async Task<Guid> Post(OnBoard onBoard)
        {
            var activationContext = FabricRuntime.GetActivationContext();

            string applicationInstance = "ServiceFabricPoc.BottleFiller";

            var serviceUri = new Uri("fabric:/" + applicationInstance + "/" + BottleFillerServiceName);

            Guid fillerId = Guid.NewGuid();

            //We create a unique Guid that is associated with a customer order, as well as with the actor that represents that order's state.
            IBottleFillerActor bottleFiller = ActorProxy.Create<IBottleFillerActor>(new ActorId(fillerId), serviceUri);

            var message = new OnBoardMessage()
            {
                Id = onBoard.Id,
                OnBoardUser = onBoard.OnBoardUser,
                SerialNumber = onBoard.SerialNumber
            };

            await bottleFiller.OnBoard(message);
            return fillerId;

        }
    }
}
