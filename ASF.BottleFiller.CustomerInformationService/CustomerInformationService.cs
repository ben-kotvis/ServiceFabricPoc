using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Domain = ServiceFabricPoc.CustomerInformation.Domain;
using Microsoft.ServiceFabric.Data;

namespace ServiceFabricPoc.CustomerInformationService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class CustomerInformationService : StatefulService
    {
        private const string CustomerInformationDictionary = "CustomerInformationDictionary";

        public CustomerInformationService(StatefulServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
        
        public async Task<IEnumerable<Domain.CustomerInformation>> GetCustomerInformationAsync(CancellationToken ct)
        {
            IList<Domain.CustomerInformation> results = new List<Domain.CustomerInformation>();
            
            IReliableDictionary<string, Domain.CustomerInformation> customers =
                await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Domain.CustomerInformation>>(CustomerInformationDictionary);

            ServiceEventSource.Current.Message("Called CustomerInformationDictionary to return CustomerInformation");            

            using (ITransaction tx = this.StateManager.CreateTransaction())
            {
                ServiceEventSource.Current.Message("Generating item views for {0} items", await customers.GetCountAsync(tx));

                IAsyncEnumerator<KeyValuePair<string, Domain.CustomerInformation>> enumerator =
                    (await customers.CreateEnumerableAsync(tx)).GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(ct))
                {
                    results.Add(enumerator.Current.Value);
                }
            }

            
            return results;
        }


        /// <summary>
        /// Used internally to generate inventory items and adds them to the ReliableDict we have.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<bool> CreateInventoryItemAsync(Domain.CustomerInformation item)
        {
            IReliableDictionary<string, Domain.CustomerInformation> inventoryItems =
                await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Domain.CustomerInformation>>(CustomerInformationDictionary);

            using (ITransaction tx = this.StateManager.CreateTransaction())
            {
                await inventoryItems.AddAsync(tx, item.Id, item);
                await tx.CommitAsync();
            }

            return true;
        }

    }
}
