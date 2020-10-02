using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace TurboPotato.NoSql
{
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class Group
    {
        public ObjectId Id { get; set; }

        public string Description { get; set; }

        public List<Person> Persons { get; set; }

        public Group()
        {
            Persons = new List<Person>();
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new MongoClient("mongodb://root:Your_password123@localhost:27017");

            var database = client.GetDatabase("mydatabase");
            await database.DropCollectionAsync("groups");

            var collection = database.GetCollection<Group>("groups");
            
            #region Simple stuff!
            await Create(collection);
            await Read(collection);
            await Update(collection);
            await Delete(collection);
            #endregion
            
            #region A bit more complex...
            await Create(collection);

            await AddPersonToGroup(collection);
            await ReadSpecificPersonFromGroup(collection);
            await RemoveSpecificPersonFromGroup(collection);
            
            await Delete(collection);
            #endregion
            
            #region Deeper down the rabbit hole...
            await Create(collection);

            //TODO: transactions?
            //TODO: nested updates?
            //TODO: aggregations?
            
            await Delete(collection);
            #endregion
        }

        private static async Task RemoveSpecificPersonFromGroup(IMongoCollection<Group> collection)
        {
            await collection.UpdateOneAsync(
                group => group.Description == "Some group description",
                Builders<Group>.Update.Pull(
                    group => group.Persons,
                    new Person()
                    {
                        Name = "Peter"
                    }));
        }

        private static async Task<Person> ReadSpecificPersonFromGroup(IMongoCollection<Group> collection)
        {
            return await collection
                .Find(
                    Builders<Group>.Filter.Eq(
                        group => group.Description,
                        "Some group description") &
                    Builders<Group>.Filter.AnyEq(
                        group => group.Persons,
                        new Person()
                        {
                            Name = "Peter"
                        }))
                .Project(group => group.Persons[-1])
                .SingleAsync();
        }

        private static async Task AddPersonToGroup(IMongoCollection<Group> collection)
        {
            await collection.UpdateOneAsync(
                group => group.Description == "Some group description",
                Builders<Group>.Update.Push(
                    x => x.Persons,
                    new Person()
                    {
                        Name = "Peter"
                    }));
        }

        private static async Task Create(IMongoCollection<Group> collection)
        {
            await collection.InsertOneAsync(new Group()
            {
                Description = "Some group description",
                Persons = new List<Person>()
                {
                    new Person()
                    {
                        Name = "John",
                        Age = 1337
                    }
                }
            });
        }

        private static async Task<Group> Read(IMongoCollection<Group> collection)
        {
            var group = await collection
                .AsQueryable()
                .Where(grp => grp.Description == "Some group description")
                .SingleAsync();

            Console.WriteLine("Read: {0}", group.Id);

            return group;
        }

        private static async Task Update(IMongoCollection<Group> collection)
        {
            await collection.UpdateOneAsync(
                group => group.Description == "Some group description",
                Builders<Group>.Update.Set(
                    group => group.Description,
                    "Some new group description"));
            
            await collection.UpdateOneAsync(
                group => group.Description == "Some new group description",
                Builders<Group>.Update.Set(
                    group => group.Description,
                    "Some group description"));
        }

        private static async Task Delete(IMongoCollection<Group> collection)
        {
            await collection.DeleteOneAsync(
                group => group.Description == "Some group description");
        }
    }
}
