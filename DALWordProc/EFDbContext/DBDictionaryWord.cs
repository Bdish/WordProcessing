using DALWordProc.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DALWordProc.EFDbContext
{
    public class DBDictionaryWord : DbContext
    {
        public DbSet<DictionaryWord> DictionaryWords { get; set; }

        public DBDictionaryWord()
        {
            //Database.EnsureDeleted();
             Database.EnsureCreated();
             Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = DBDictionaryWord; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False");
        }

    }
    
}
