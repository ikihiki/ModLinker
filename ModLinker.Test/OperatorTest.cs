using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DokanNet;
using Moq;
using Xunit;

namespace ModLinker.Test
{
    public class OperatorTest
    {

        [Fact]
        public async Task CreateLayer()
        {
            var layerMoq = new Mock<ILayer>();
            layerMoq.SetupGet(layer => layer.Id).Returns(Guid.NewGuid().ToString());
            layerMoq.Setup(layer => layer.GetPreloadEntries())
                .Returns(new[]
                {
                    new EntryData(
                        true, 
                        "dir5",
                        @"D:\dir3\dir4\dir5",
                        @"D:\dir3\dir4",
                        true,
                        layerMoq.Object,
                        DateTime.Now, 
                        DateTime.Now, 
                        DateTime.Now, 
                        0,
                        false,
                        1
                        )
                });
            layerMoq.Setup(layer=>layer.GetEntries(@"D:\dir3\dir4\dir5"))
                .Returns(new[]
                {
                    new EntryData(
                        true,
                        "file1",
                        @"D:\dir3\dir4\dir5\file1",
                        @"D:\dir3\dir4\dir5",
                        false,
                        layerMoq.Object,
                        DateTime.Now,
                        DateTime.Now,
                        DateTime.Now,
                        10,
                        false,
                        1
                    )
                });

            var entryMoq = new Mock<IEntry>();
            entryMoq.SetupGet(entry => entry.CreationTime).Returns(DateTime.Now);
            entryMoq.SetupGet(entry => entry.LastAccessTime).Returns(DateTime.Now);
            entryMoq.SetupGet(entry => entry.LastWriteTime).Returns(DateTime.Now);
            entryMoq.SetupGet(entry => entry.Name).Returns("file1");
            entryMoq.SetupGet(entry => entry.Length).Returns(10);

            layerMoq.Setup(layer => layer.GetEntry(@"D:\dir3\dir4\dir5\file1"))
                .Returns(entryMoq.Object)
                ;
            var service = new LayerService(new[] {layerMoq.Object}, @"D:\dir3\dir4");
            await service.PreLoad();
            var operation = new ModLinkerOperation(service);
            FileInformation fileInformation;
            var result = operation.GetFileInformation(@"D:\dir3\dir4\dir5\file1", out fileInformation, new TestFileInfo());
            Assert.Equal(DokanResult.Success, result);
            Assert.Equal("file1", fileInformation.FileName);
        }
    }
}
