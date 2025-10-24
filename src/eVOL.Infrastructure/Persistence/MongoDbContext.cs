using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVOL.Domain.Entities;
using MongoDB.Driver;

namespace eVOL.Infrastructure.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string dbName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(dbName);
        }

        public IMongoCollection<ChatMessage> ChatMessages =>
            _database.GetCollection<ChatMessage>("ChatMessages");
    }
}
