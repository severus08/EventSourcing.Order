using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace Order.Infrastructure.EventStore
{
    public class EventStoreAdapter : IEventStoreConnection
    {
        private readonly IEventStoreConnection _eventStoreConnectionImplementation;

        public EventStoreAdapter(string uri, string userName, string password)
        {
            if (_eventStoreConnectionImplementation == default(IEventStoreConnection))
            {
                _eventStoreConnectionImplementation =
                    EventStoreConnection.Create(new Uri($"tcp://{userName}:{password}@{uri}"));
                _eventStoreConnectionImplementation.ConnectAsync().Wait();
            }
        }

        public void Dispose()
        {
            _eventStoreConnectionImplementation.Dispose();
        }

        public async Task ConnectAsync()
        {
            await _eventStoreConnectionImplementation.ConnectAsync();
        }

        public void Close()
        {
            _eventStoreConnectionImplementation.Close();
        }

        public async Task<DeleteResult> DeleteStreamAsync(string stream, long expectedVersion,
            UserCredentials userCredentials = null)
        {
            return await _eventStoreConnectionImplementation.DeleteStreamAsync(stream, expectedVersion,
                userCredentials);
        }

        public async Task<DeleteResult> DeleteStreamAsync(string stream, long expectedVersion, bool hardDelete,
            UserCredentials userCredentials = null)
        {
            return await _eventStoreConnectionImplementation.DeleteStreamAsync(stream, expectedVersion, hardDelete,
                userCredentials);
        }

        public async Task<WriteResult> AppendToStreamAsync(string stream, long expectedVersion,
            params global::EventStore.ClientAPI.EventData[] events)
        {
            return await _eventStoreConnectionImplementation.AppendToStreamAsync(stream, expectedVersion, events);
        }

        public async Task<WriteResult> AppendToStreamAsync(string stream, long expectedVersion,
            UserCredentials userCredentials,
            params global::EventStore.ClientAPI.EventData[] events)
        {
            return await _eventStoreConnectionImplementation.AppendToStreamAsync(stream, expectedVersion,
                userCredentials, events);
        }

        public async Task<WriteResult> AppendToStreamAsync(string stream, long expectedVersion,
            IEnumerable<global::EventStore.ClientAPI.EventData> events,
            UserCredentials userCredentials = null)
        {
            return await _eventStoreConnectionImplementation.AppendToStreamAsync(stream, expectedVersion, events,
                userCredentials);
        }

        public async Task<ConditionalWriteResult> ConditionalAppendToStreamAsync(string stream, long expectedVersion,
            IEnumerable<global::EventStore.ClientAPI.EventData> events,
            UserCredentials userCredentials = null)
        {
            return await _eventStoreConnectionImplementation.ConditionalAppendToStreamAsync(stream, expectedVersion,
                events, userCredentials);
        }

        public async Task<EventStoreTransaction> StartTransactionAsync(string stream, long expectedVersion,
            UserCredentials userCredentials = null)
        {
            return await _eventStoreConnectionImplementation.StartTransactionAsync(stream, expectedVersion,
                userCredentials);
        }

        public EventStoreTransaction ContinueTransaction(long transactionId, UserCredentials userCredentials = null)
        {
            return _eventStoreConnectionImplementation.ContinueTransaction(transactionId, userCredentials);
        }

        public async Task<EventReadResult> ReadEventAsync(string stream, long eventNumber, bool resolveLinkTos,
            UserCredentials userCredentials = null)
        {
            return await _eventStoreConnectionImplementation.ReadEventAsync(stream, eventNumber, resolveLinkTos,
                userCredentials);
        }

        public async Task<StreamEventsSlice> ReadStreamEventsForwardAsync(string stream, long start, int count,
            bool resolveLinkTos,
            UserCredentials userCredentials = null)
        {
            return await _eventStoreConnectionImplementation.ReadStreamEventsForwardAsync(stream, start, count,
                resolveLinkTos, userCredentials);
        }

        public async Task<StreamEventsSlice> ReadStreamEventsBackwardAsync(string stream, long start, int count,
            bool resolveLinkTos,
            UserCredentials userCredentials = null)
        {
            return await _eventStoreConnectionImplementation.ReadStreamEventsBackwardAsync(stream, start, count,
                resolveLinkTos, userCredentials);
        }

        public async Task<AllEventsSlice> ReadAllEventsForwardAsync(Position position, int maxCount,
            bool resolveLinkTos,
            UserCredentials userCredentials = null)
        {
            return await _eventStoreConnectionImplementation.ReadAllEventsForwardAsync(position, maxCount,
                resolveLinkTos, userCredentials);
        }

        public async Task<AllEventsSlice> ReadAllEventsBackwardAsync(Position position, int maxCount,
            bool resolveLinkTos,
            UserCredentials userCredentials = null)
        {
            return await _eventStoreConnectionImplementation.ReadAllEventsBackwardAsync(position, maxCount,
                resolveLinkTos, userCredentials);
        }

        public async Task<EventStoreSubscription> SubscribeToStreamAsync(string stream, bool resolveLinkTos,
            Func<EventStoreSubscription, ResolvedEvent, Task> eventAppeared,
            Action<EventStoreSubscription, SubscriptionDropReason, Exception> subscriptionDropped = null,
            UserCredentials userCredentials = null)
        {
            return await _eventStoreConnectionImplementation.SubscribeToStreamAsync(stream, resolveLinkTos,
                eventAppeared, subscriptionDropped, userCredentials);
        }

        public EventStoreStreamCatchUpSubscription SubscribeToStreamFrom(string stream, long? lastCheckpoint,
            CatchUpSubscriptionSettings settings,
            Func<EventStoreCatchUpSubscription, ResolvedEvent, Task> eventAppeared,
            Action<EventStoreCatchUpSubscription> liveProcessingStarted = null,
            Action<EventStoreCatchUpSubscription, SubscriptionDropReason, Exception> subscriptionDropped = null,
            UserCredentials userCredentials = null)
        {
            return _eventStoreConnectionImplementation.SubscribeToStreamFrom(stream, lastCheckpoint, settings,
                eventAppeared, liveProcessingStarted, subscriptionDropped, userCredentials);
        }

        public async Task<EventStoreSubscription> SubscribeToAllAsync(bool resolveLinkTos,
            Func<EventStoreSubscription, ResolvedEvent, Task> eventAppeared,
            Action<EventStoreSubscription, SubscriptionDropReason, Exception> subscriptionDropped = null,
            UserCredentials userCredentials = null)
        {
            return await _eventStoreConnectionImplementation.SubscribeToAllAsync(resolveLinkTos, eventAppeared,
                subscriptionDropped, userCredentials);
        }

        public EventStorePersistentSubscriptionBase ConnectToPersistentSubscription(string stream, string groupName,
            Func<EventStorePersistentSubscriptionBase, ResolvedEvent, int?, Task> eventAppeared,
            Action<EventStorePersistentSubscriptionBase, SubscriptionDropReason, Exception> subscriptionDropped = null,
            UserCredentials userCredentials = null, int bufferSize = 10,
            bool autoAck = true)
        {
            return _eventStoreConnectionImplementation.ConnectToPersistentSubscription(stream, groupName, eventAppeared,
                subscriptionDropped, userCredentials, bufferSize, autoAck);
        }

        public async Task<EventStorePersistentSubscriptionBase> ConnectToPersistentSubscriptionAsync(string stream,
            string groupName, Func<EventStorePersistentSubscriptionBase, ResolvedEvent, int?, Task> eventAppeared,
            Action<EventStorePersistentSubscriptionBase, SubscriptionDropReason, Exception> subscriptionDropped = null,
            UserCredentials userCredentials = null, int bufferSize = 10,
            bool autoAck = true)
        {
            return await _eventStoreConnectionImplementation.ConnectToPersistentSubscriptionAsync(stream, groupName,
                eventAppeared, subscriptionDropped, userCredentials, bufferSize, autoAck);
        }

        public EventStoreAllCatchUpSubscription SubscribeToAllFrom(Position? lastCheckpoint,
            CatchUpSubscriptionSettings settings,
            Func<EventStoreCatchUpSubscription, ResolvedEvent, Task> eventAppeared,
            Action<EventStoreCatchUpSubscription> liveProcessingStarted = null,
            Action<EventStoreCatchUpSubscription, SubscriptionDropReason, Exception> subscriptionDropped = null,
            UserCredentials userCredentials = null)
        {
            return _eventStoreConnectionImplementation.SubscribeToAllFrom(lastCheckpoint, settings, eventAppeared,
                liveProcessingStarted, subscriptionDropped, userCredentials);
        }

        public async Task UpdatePersistentSubscriptionAsync(string stream, string groupName,
            PersistentSubscriptionSettings settings,
            UserCredentials credentials)
        {
            await _eventStoreConnectionImplementation.UpdatePersistentSubscriptionAsync(stream, groupName, settings,
                credentials);
        }

        public async Task CreatePersistentSubscriptionAsync(string stream, string groupName,
            PersistentSubscriptionSettings settings,
            UserCredentials credentials)
        {
            await _eventStoreConnectionImplementation.CreatePersistentSubscriptionAsync(stream, groupName, settings,
                credentials);
        }

        public async Task DeletePersistentSubscriptionAsync(string stream, string groupName,
            UserCredentials userCredentials = null)
        {
            await _eventStoreConnectionImplementation.DeletePersistentSubscriptionAsync(stream, groupName,
                userCredentials);
        }

        public async Task<WriteResult> SetStreamMetadataAsync(string stream, long expectedMetastreamVersion,
            StreamMetadata metadata,
            UserCredentials userCredentials = null)
        {
            return await _eventStoreConnectionImplementation.SetStreamMetadataAsync(stream, expectedMetastreamVersion,
                metadata, userCredentials);
        }

        public async Task<WriteResult> SetStreamMetadataAsync(string stream, long expectedMetastreamVersion,
            byte[] metadata,
            UserCredentials userCredentials = null)
        {
            return await _eventStoreConnectionImplementation.SetStreamMetadataAsync(stream, expectedMetastreamVersion,
                metadata, userCredentials);
        }

        public async Task<StreamMetadataResult> GetStreamMetadataAsync(string stream,
            UserCredentials userCredentials = null)
        {
            return await _eventStoreConnectionImplementation.GetStreamMetadataAsync(stream, userCredentials);
        }

        public async Task<RawStreamMetadataResult> GetStreamMetadataAsRawBytesAsync(string stream,
            UserCredentials userCredentials = null)
        {
            return await _eventStoreConnectionImplementation.GetStreamMetadataAsRawBytesAsync(stream, userCredentials);
        }

        public async Task SetSystemSettingsAsync(SystemSettings settings, UserCredentials userCredentials = null)
        {
            await _eventStoreConnectionImplementation.SetSystemSettingsAsync(settings, userCredentials);
        }

        public string ConnectionName => _eventStoreConnectionImplementation.ConnectionName;

        public ConnectionSettings Settings => _eventStoreConnectionImplementation.Settings;

        public event EventHandler<ClientConnectionEventArgs> Connected
        {
            add => _eventStoreConnectionImplementation.Connected += value;
            remove => _eventStoreConnectionImplementation.Connected -= value;
        }

        public event EventHandler<ClientConnectionEventArgs> Disconnected
        {
            add => _eventStoreConnectionImplementation.Disconnected += value;
            remove => _eventStoreConnectionImplementation.Disconnected -= value;
        }

        public event EventHandler<ClientReconnectingEventArgs> Reconnecting
        {
            add => _eventStoreConnectionImplementation.Reconnecting += value;
            remove => _eventStoreConnectionImplementation.Reconnecting -= value;
        }

        public event EventHandler<ClientClosedEventArgs> Closed
        {
            add => _eventStoreConnectionImplementation.Closed += value;
            remove => _eventStoreConnectionImplementation.Closed -= value;
        }

        public event EventHandler<ClientErrorEventArgs> ErrorOccurred
        {
            add => _eventStoreConnectionImplementation.ErrorOccurred += value;
            remove => _eventStoreConnectionImplementation.ErrorOccurred -= value;
        }

        public event EventHandler<ClientAuthenticationFailedEventArgs> AuthenticationFailed
        {
            add => _eventStoreConnectionImplementation.AuthenticationFailed += value;
            remove => _eventStoreConnectionImplementation.AuthenticationFailed -= value;
        }
    }
}