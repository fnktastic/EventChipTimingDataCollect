using ReaderDataCollector.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.DataAccess
{
    public class Context : DbContext
    {
        public Context() : base("LocalAtwDB") { }

        public DbSet<Read> Reads { get; set; }
        public DbSet<Reading> Readings { get; set; }
        public DbSet<Reader> Readers { get; set; }
        public DbSet<Setting> Settings { get; set; }
    }

    public class Initializer : DropCreateDatabaseIfModelChanges<Context>
    {
        public Initializer()
        {
            using (var context = new Context())
            {
                InitializeDatabase(context);
            }
        }
    }
}
