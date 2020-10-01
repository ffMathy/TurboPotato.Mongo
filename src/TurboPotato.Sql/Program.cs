using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TurboPotato.Sql
{
    public class User
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
            await using var context = new DatabaseContext();
        }

        private static void Create(DatabaseContext context)
        {

        }
    }
}
