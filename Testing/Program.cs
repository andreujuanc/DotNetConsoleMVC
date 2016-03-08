using ConsoleHVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new TestContext();
            if(db.SomeReferenceProperty != null) db.SomeReferenceProperty.ID.ToString();
            db.SomeReferenceProperty = new SomeClass() { ID = 99 };
            db.SaveChanges();
        }
    }
    public class TestContext : ConsoleDbContext
    {
        public SomeClass SomeReferenceProperty { get { return Get<SomeClass>(); } set { Set<SomeClass>(value); } }

        public override void OnConfiguring(ConsoleDbContextOptions options)
        {
            options.DataFile = "Test.data";
        }
    }
    public class SomeClass { public int ID { get; set; } }
}
