using MongoDB.Bson;
using MongoDB.Driver; 
using System;
using System.Collections.Generic;
using System.Text;

namespace OtoRobotWeb2.Models.MongoDb
{
    public class logTimeCollection
    {
        internal MongoDBRespository _repo = new MongoDBRespository();
        public IMongoCollection<logTime> Collection;

        public logTimeCollection(string kodu)
        {
            // here we were created new Collection with Products name
            this.Collection = _repo.db.GetCollection<logTime>("logTimes" + kodu);
        }
        public void InsertContact(logTime contact)
        {
            this.Collection.InsertOneAsync(contact);
        }

        public List<logTime> GetAllLogTime()
        {
            var query = this.Collection.Find(new BsonDocument()).ToListAsync();
            return query.Result;
        }
    }
}
