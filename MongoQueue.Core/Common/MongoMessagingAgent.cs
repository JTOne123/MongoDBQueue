﻿using MongoDB.Driver;
using MongoQueue.Core.Read;

namespace MongoQueue.Core.Common
{
    public class MongoMessagingAgent
    {
        private readonly IMessagingConfiguration _messagingConfiguration;
        private readonly string _dbName;

        public MongoMessagingAgent(
            IMessagingConfiguration messagingConfiguration
        )
        {
            _messagingConfiguration = messagingConfiguration;
            _dbName = MongoUrl.Create(_messagingConfiguration.ConnectionString).DatabaseName;
        }

        public IMongoDatabase GetDb()
        {
            var mongoUrl = MongoUrl.Create(_messagingConfiguration.ConnectionString);
            var settings = MongoClientSettings.FromUrl(mongoUrl);
            settings.WriteConcern = WriteConcern.Acknowledged;
            var client = new MongoClient(settings);
            return client.GetDatabase(_dbName);
        }

        public IMongoCollection<TDocument> GetCollection<TDocument>(string name = null)
        {
            name = name ?? typeof(TDocument).Name;
            var db = GetDb();
            return db.GetCollection<TDocument>(name, new MongoCollectionSettings
            {
                WriteConcern = WriteConcern.Acknowledged,
                ReadConcern = ReadConcern.Default
            });
        }

        public IMongoCollection<Envelope> GetEnvelops(string appName)
        {
            return GetCollection<Envelope>(GetEnvelopsCollectionName(appName));
        }

        public string GetEnvelopsCollectionName(string appName)
        {
            return $"{appName}_Envelops";
        }
    }
}