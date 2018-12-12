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
        public Context() : base("reads") { }

        public DbSet<Read> Reads { get; set; }
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
