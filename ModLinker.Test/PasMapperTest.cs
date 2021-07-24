using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ModLinker.Test
{
    public class PasMapperTest
    {
        [Fact]
        public void MappedPathTest()
        {
            var mapper = new PathMapper(@"C:\dir1\dir2", @"D:\dir3\dir4");

            var result = mapper.GetMappedPath(@"C:\dir1\dir2\dir5\file1.txt");
            Assert.Equal(@"D:\dir3\dir4\dir5\file1.txt", result);
        }

        [Fact]
        public void SourcePathTest()
        {
            var mapper = new PathMapper(@"C:\dir1\dir2", @"D:\dir3\dir4");

            var result = mapper.GetSourcePath(@"D:\dir3\dir4\dir5\file1.txt");
            Assert.Equal(@"C:\dir1\dir2\dir5\file1.txt", result);
        }

        [Fact]
        public void GetPathFragmentsFromTargetPathTest()
        {

            var mapper = new PathMapper(@"C:\dir1\dir2", @"D:\dir3\dir4");

            var result = mapper.GetPathFragmentsFromTargetPath(@"D:\dir3\dir4\dir5\dir6\file1.txt");


            Assert.Equal(new[]
            {
                @"D:\dir3\dir4\dir5",
                @"D:\dir3\dir4\dir5\dir6",
                @"D:\dir3\dir4\dir5\dir6\file1.txt",
            },
                result);
        }
    }
}
