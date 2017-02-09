using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace ServiceFabricPoc.BottleFiller.Domain
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IBottleFillerActor : IActor
    {
        Task<BottleFillerStatistics> GetCurrentStatistics();

        Task<BottleFillerStatistics> OnBoard(OnBoardMessage message);

        Task AddUsage(AddUsageMessage message);
    }
}
