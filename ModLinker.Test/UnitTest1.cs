using System;
using Xunit;

namespace ModLinker.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var builder = new DatabaseBuilder();
            builder.Append(new[]{
                new Directory{Path="/Deps1"},
                new Directory{Path="/Deps1/Deps2"},
                new Directory{Path="/Deps1/Deps2/Deps3"},
                new Directory{Path="/Deps1/Deps2/Deps3-2"}
            });
            var db = new MemoryDatabase(builder.Build());
            var result = db.DirectoryTable.FindRangeByPath("/Deps1","/Deps1/Deps2/Deps3-2");
            Assert.Equal(result.Count, 4);
        }
    }
}