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

    public class BottleFillerStatisticsController: ApiController
    {
        private const string BottleFillerServiceName = "BottleFillerActorService";

        [HttpGet]
        [Route("api/bottlefillerstatistics/{fillerId}")]
        public async Task<Model.BottleFillerStatistics> GetStatistics(Guid fillerId)
        {
            var activationContext = FabricRuntime.GetActivationContext();

            string applicationInstance = "ServiceFabricPoc.BottleFiller";

            var serviceUri = new Uri("fabric:/" + applicationInstance + "/" + BottleFillerServiceName);            

            //We create a unique Guid that is associated with a customer order, as well as with the actor that represents that order's state.
            IBottleFillerActor bottleFiller = ActorProxy.Create<IBottleFillerActor>(new ActorId(fillerId), serviceUri);                 

            var stats = await bottleFiller.GetCurrentStatistics();

            return new Model.BottleFillerStatistics()
            {
                CurrentFilterGallonsUsed = stats.CurrentFilterGallonsUsed,
                FilterInstallDate = stats.FilterInstallDate,
                Id = stats.Id, 
                OnBoardUser= stats.OnBoardUser, 
                SerialNumber = stats.SerialNumber,
                ServiceDate = stats.ServiceDate,
                TotalGallons = stats.TotalGallons
            };

        }
    }
}
