using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functions
{
    public class ConnectionDb:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=OrmDb;Username=postgres;password=mukavina123;");
        }
        public DbSet<Personal> Personals { get; set; }
        public class Personal
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Surname { get; set; }
        }
    }
}
