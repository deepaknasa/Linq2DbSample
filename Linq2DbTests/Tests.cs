using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LinqToDB.Mapping;
using LinqToDB;
using LinqToDB.Data;
using System.Diagnostics;
using System.Linq;

namespace Linq2DbTests
{
    [Table]
    public class TestTable
    {
        [PrimaryKey, Identity] public int ID;
        [NotNull, Column] public string Name;
        [Nullable, Column] public string Description;
    }

    [Table]
    public class TestTable2
    {
        [PrimaryKey, Identity] public int ID;
        [NotNull, Column] public string Name;
        [Nullable, Column] public string Description;
    }

    [TestClass]
    public class Tests
    {
        const string dbConnString = ProviderName.SqlServer;

        public Tests()
        {
            DataConnection.TurnTraceSwitchOn();
            DataConnection.WriteTraceLine = (s, s1) => Debug.WriteLine(s, s1);
        }



        [ClassInitialize]
        public static void Init(TestContext context)
        {
            using (var db = new DataConnection(dbConnString))
            {
                try {
                    db.DropTable<TestTable>();
                    db.DropTable<TestTable2>();
                } catch (Exception) { }
                db.CreateTable<TestTable>();
                db.CreateTable<TestTable2>();
            }
        }

        [TestMethod]
        public void InsertWithNewTest()
        {
            using (var db = new DataConnection(dbConnString))
            {
                db.Insert(new TestTable {
                    Name = "Data 1"
                });
            }
        }

        [TestMethod]
        public void InsertWithExpressionTest()
        {
            using (var db = new DataConnection(dbConnString))
            {
                db.GetTable<TestTable>()
                    .Insert(() => new TestTable
                    {
                        Name = "Data 1 with expression"
                    });
            }
        }

        [TestMethod]
        public void InsertIntoSelectTest()
        {
            using (var db = new DataConnection(dbConnString))
            {
                db.GetTable<TestTable>()
                    .Where(tb => tb.ID == 1)
                    .Insert(
                    db.GetTable<TestTable2>(),
                    (t) => new TestTable2
                    {
                        Name = t.Name + " -II"
                    });
            }
        }

        [TestMethod]
        public void InsertIntoTest()
        {
            using (var db = new DataConnection(dbConnString))
            {
                db.GetTable<TestTable>()
                    .Where(t => t.ID == 1)
                    .Into(db.GetTable<TestTable2>())
                        .Value(t => t.Name, t => t.Name + " -II")
                    .Insert();
            }
        }

        [TestMethod]
        public void BulkCopyTest()
        {
            using (var db = new DataConnection(dbConnString))
            {
                db.BulkCopy(Enumerable
                    .Range(1, 100)
                    .Select(n => new TestTable {
                        Name = n.ToString()
                    }));
            }
        }
    }
}
