using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace OtoRobotWeb2.Models.MongoDb
{
    public class MongoDBRespository
    {
        // MongoClient is used for connect to server 
        public MongoClient client;
        // IMongoDatabase interface is used for database transactions
        public IMongoDatabase db;

        public MongoDBRespository()
        {
            var settings = MongoClientSettings.FromConnectionString("mongodb+srv://neosinerji:Kayapar0099@cluster0.uc8sknk.mongodb.net/?retryWrites=true&w=majority");
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);

            //here we are connected to server 
            this.client = new MongoClient(settings);
            // if database is not exist we create new one with ExampleMongoDB name
            this.db = this.client.GetDatabase("log_OtoRobot");
        }
    }
}
