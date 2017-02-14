using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using ServiceFabricPoc.BottleFiller.Domain;
using ServiceFabricPoc.EventWriter.Actors.Domain;

namespace ServiceFabricPoc.BottleFiller.Actors
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class BottleFillerActor : Actor, IBottleFillerActor
    {

        private const string BottleFillerStatusPropertyName = "BottleFillerStatus";
        private const string BottleFillerStatisticsPropertyName = "BottleFillerStatistics";
        private const string BottleFillerStatisticsHoldingPropertyName = "BottleFillerStatisticsHolding";

        /// <summary>
        /// Initializes a new instance of Actor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public BottleFillerActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization

            return Task.FromResult(true);
        }

        public async Task<BottleFillerStatistics> GetCurrentStatistics()
        {
            return await this.StateManager.GetStateAsync<BottleFillerStatistics>(BottleFillerStatisticsPropertyName);
        }

        public async Task AddUsage(AddUsageMessage message)
        {
            var currentStatistics = await this.StateManager.GetStateAsync<BottleFillerStatistics>(BottleFillerStatisticsPropertyName);
            
            currentStatistics.TotalGallons += message.NumberOfGallons;
            currentStatistics.CurrentFilterGallonsUsed += message.NumberOfGallons;
            await this.StateManager.SetStateAsync(BottleFillerStatisticsPropertyName, currentStatistics);

            try
            {

                string applicationInstance = "ServiceFabricPoc.BottleFiller";

                var serviceUri = new Uri("fabric:/" + applicationInstance + "/EventWriterActorService");
                IEventWriterActor bottleFiller = ActorProxy.Create<IEventWriterActor>(new ActorId(Guid.NewGuid()), serviceUri);

                var eventMessage = new UsageAddedEventMessage()
                {
                    ActorId = this.Id.ToString(),
                    EventTime = DateTimeOffset.UtcNow,
                    NumberOfGallons = message.NumberOfGallons
                };

                await bottleFiller.Write(eventMessage, "usageaddedeventstream");
            }
            catch(Exception ex)
            {
                currentStatistics.TotalGallons -= message.NumberOfGallons;
                currentStatistics.CurrentFilterGallonsUsed -= message.NumberOfGallons;

                await this.StateManager.SetStateAsync<BottleFillerStatistics>(BottleFillerStatisticsPropertyName, currentStatistics);

                throw;
            }

        }

        public async Task<BottleFillerStatistics> OnBoard(OnBoardMessage message)
        {
            var statistics = new BottleFillerStatistics()
            {
                Id = message.Id,
                SerialNumber = message.SerialNumber,
                OnBoardUser = message.OnBoardUser,
                CurrentFilterGallonsUsed = 0,
                FilterInstallDate = DateTimeOffset.UtcNow,
                ServiceDate = DateTimeOffset.UtcNow,
                TotalGallons = 0
            };

            await this.StateManager.SetStateAsync<BottleFillerStatistics>(BottleFillerStatisticsPropertyName, statistics);

            await SetOrderStatusAsync(BottleFillerStatus.OnBoarded);

            return statistics;
        }
        
        private async Task SetOrderStatusAsync(BottleFillerStatus fillerStatus)
        {
            await this.StateManager.SetStateAsync<BottleFillerStatus>(BottleFillerStatusPropertyName, fillerStatus);
        }
    }
}
