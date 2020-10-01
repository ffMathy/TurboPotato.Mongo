using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TurboPotato.Sql
{
    public class Person
    {
        public Guid Id { get; set; }

        public Group Group { get; set; }
        public Guid GroupId { get; set; }

        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class Group
    {
        public Guid Id { get; set; }

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
            await using var context = new DatabaseContext();
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            
            #region Simple stuff!
            await Create();
            await Read();
            await Update();
            await Delete();
            #endregion

            #region A bit more complex...
            await Create();

            await AddPersonToGroup();
            await ReadSpecificPersonFromGroup();
            await RemoveSpecificPersonFromGroup();

            await Delete();
            #endregion

            #region Deeper down the rabbit hole...
            await Create();
            
            //TODO: transactions?
            //TODO: nested updates?
            //TODO: aggregations?
            
            await Delete();
            #endregion
        }

        private static async Task RemoveSpecificPersonFromGroup()
        {
            await using var context = new DatabaseContext();

            var person = await context.Persons.SingleAsync(p => 
                p.Group.Description == "Some group description" &&
                p.Name == "John");
            context.Persons.Remove(person);

            await context.SaveChangesAsync();
        }

        private static async Task<Person> ReadSpecificPersonFromGroup()
        {
            await using var context = new DatabaseContext();

            return await context.Persons
                .SingleAsync(person => 
                    person.Group.Description == "Some group description" &&
                    person.Name == "John");
        }

        private static async Task AddPersonToGroup()
        {
            await using var context = new DatabaseContext();

            var group = await context.Groups
                .SingleAsync(grp => grp.Description == "Some group description");
            group.Persons.Add(new Person()
            {
                Name = "Peter"
            });

            await context.SaveChangesAsync();
        }

        private static async Task Create()
        {
            await using var context = new DatabaseContext();

            await context.Groups.AddAsync(new Group()
            {
                Description = "Some group description",
                Persons = new List<Person>() {
                    new Person() {
                        Name = "John"
                    }
                }
            });
            await context.SaveChangesAsync();
        }

        private static async Task<Group> Read()
        {
            await using var context = new DatabaseContext();

            var group = await context.Groups
                .SingleAsync(grp => grp.Description == "Some group description");

            Console.WriteLine("Read: {0}", group.Id);

            return group;
        }

        private static async Task Update()
        {
            await using var context = new DatabaseContext();

            var group = await context.Groups
                .SingleAsync(grp => grp.Description == "Some group description");

            group.Description = "Some new group description";
            await context.SaveChangesAsync();
            
            group.Description = "Some group description";
            await context.SaveChangesAsync();
        }

        private static async Task Delete()
        {
            await using var context = new DatabaseContext();

            var group = await context.Groups
                .SingleAsync(grp => grp.Description == "Some group description");
            context.Groups.Remove(group);

            await context.SaveChangesAsync();
        }
    }
}
