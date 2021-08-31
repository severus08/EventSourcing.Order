using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EventStore.ClientAPI.Common.Log;
using EventStore.ClientAPI.Projections;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;
using Order.Infrastructure.AppSettingsConfigration;

namespace Order.Infrastructure.EventStore
{
    public class EventStoreProjectionAdapter
    {
        private ProjectionsManager _projectionsManager;
        

        private readonly UserCredentials _adminCredentials = new UserCredentials(EventStoreConfigration.EventStoreConfig.UserName,
            EventStoreConfigration.EventStoreConfig.Password);

        public EventStoreProjectionAdapter(string uri, int port)
        {
            if (_projectionsManager == default)
            {
                _projectionsManager = new ProjectionsManager(
                    log: new ConsoleLogger(),
                    // httpEndPoint: new IPEndPoint(IPAddress.Parse(uri), port),
                    httpEndPoint:new DnsEndPoint(uri,port),
                    operationTimeout: TimeSpan.FromMilliseconds(5000)
                );
                
            }
        }

        public async Task<List<string>> ListAll()
        {
            return (await _projectionsManager.ListAllAsync(_adminCredentials)).Select(x => x.Name).ToList();
        }

        public async Task UpsertCustomProjections(Dictionary<string,string> registeredProjections)
        {
            var projections = (await _projectionsManager.ListAllAsync(_adminCredentials)).Select(x => x.Name).ToList();

            foreach (var proj in registeredProjections.ToList())
            {
                if (projections.Contains(proj.Key))
                {
                    // await _projectionsManager.DisableAsync(proj.Key, _adminCredentials);
                    // await _projectionsManager.DeleteAsync(proj.Key, true,
                    //     _adminCredentials);
                    await _projectionsManager.ResetAsync(proj.Key, _adminCredentials);
                    continue;
                }

                await _projectionsManager.CreateContinuousAsync(
                    name: proj.Key,
                    query: proj.Value,
                    trackEmittedStreams: true,
                    userCredentials: _adminCredentials
                );
            }            
        }

        public async Task<T> GetPartitonState<T>(string name, string streamName, Guid id)
        {
            var state = await _projectionsManager.GetPartitionStateAsync(name
                , $"{streamName}-{id.ToString()}",
                _adminCredentials);
            if (string.IsNullOrEmpty(state))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(state);
        }

        public async Task<T> GetPartitonResult<T>(string name, string streamName, Guid id)
        {
            var result = await _projectionsManager.GetPartitionResultAsync(name
                , $"{streamName}-{id.ToString()}",
                _adminCredentials);
            if (string.IsNullOrEmpty(result))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(result);
        }
    }
}