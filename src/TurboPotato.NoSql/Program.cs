using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TurboPotato.NoSql
{
    public class User
    {
        public ObjectId Id { get; set; }

        public Group Group { get; set; }
        public Guid GroupId { get; set; }

        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class Group
    {
        public ObjectId Id { get; set; }

        public List<User> Users { get; set; }

        public Group()
        {
            Users = new List<User>();
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new MongoClient("mongodb://root:Your_password123@localhost:27017");
            Create(client);
        }

        private static void Create(MongoClient client)
        {

        }
    }
}
