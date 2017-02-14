using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using ServiceFabricPoc.EventWriter.Actors.Domain;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System.Text;

namespace ServiceFabricPoc.EventWriter.Actors
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.None)]
    internal class EventWriterActor : Actor, IEventWriterActor
    {

        public const string connectionString = "Endpoint=sb://elkay-inbound.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=eVC60er6ahstlcu9gI2/KoQfCreQ5sYgMlYPSuFe4kQ=";
        /// <summary>
        /// Initializes a new instance of Actors
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public EventWriterActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }
        
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            return this.StateManager.TryAddStateAsync("count", 0);
        }
        public async Task Write(EventMessage messageObject, string hubName)
        {
            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, hubName);

            var message = JsonConvert.SerializeObject(messageObject);
            int retryCount = 0;

            while (retryCount < 3)
            {
                try
                {
                    await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if(retryCount == 2)
                    {
                        throw;
                    }
                }
            }

        }
    }
}
