using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Order.Infrastructure.AppSettingsConfigration;

namespace Order.Infrastructure.Mongo
{
    public class MongoRepository : IMongoRepository
    {
        private protected IMongoClient _client;
        private protected IMongoDatabase _database;

        public MongoRepository()
        {
            var mongoUrlBuilder = new MongoUrlBuilder(MongoAppConfigration.MongoConfig.ConnectionString);
            var settings = MongoClientSettings.FromUrl(mongoUrlBuilder.ToMongoUrl());
            _client = new MongoClient(settings);
            _database = _client.GetDatabase(mongoUrlBuilder.DatabaseName);
        }

        private IMongoCollection<T> Collection<T>(string collectionName)
            => _database.GetCollection<T>(collectionName ?? typeof(T).Name);

        public async Task<List<T>> SearchAsync<T>(Expression<Func<T, bool>> expression, int skip = 0, int limit = 100,
            string collectionName = null) where T : class, new()
            => await Collection<T>(collectionName).Find(expression).Skip(skip).Limit(limit).ToListAsync();

        public List<T> Search<T>(Expression<Func<T, bool>> expression, int skip = 0, int limit = 100,
            string collectionName = null) where T : class, new()
            => Collection<T>(collectionName).Find(expression).Skip(skip).Limit(limit).ToList();

        public async Task<T> GetAsync<T>(Expression<Func<T, bool>> expression, string collectionName = null)
            where T : class, new()
            => await Collection<T>(collectionName ?? typeof(T).Name).Find(expression).FirstOrDefaultAsync();

        public async Task InsertOneAsync<T>(T item, string collectionName = null) where T : class, new()
            => await Collection<T>(collectionName ?? typeof(T).Name).InsertOneAsync(item);

        public async Task UpdateOneAsync<T>(Expression<Func<T, bool>> expression, T item, string collectionName = null)
            where T : class, new()
            => await Collection<T>(collectionName ?? typeof(T).Name).ReplaceOneAsync(expression, item);
        public async Task DeleteManyAsync<T>(Expression<Func<T, bool>> expression, string collectionName = null)
            where T : class, new()
            => await Collection<T>(collectionName ?? typeof(T).Name).DeleteManyAsync(expression);
        
        
    }
}