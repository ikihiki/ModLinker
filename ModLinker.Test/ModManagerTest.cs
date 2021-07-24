using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DokanNet;
using Xunit;
using Moq;

namespace ModLinker.Test
{
    public class ModManagerTest
    {
        [Fact]
        public async Task LoadTest()
        {
            var layerProviderMoq = new Mock<ILayerProvider>();
            var layerMoq = new Mock<ILayer>();
            layerMoq.SetupGet(layer => layer.Id).Returns(Guid.NewGuid().ToString());
            layerMoq.Setup(layer => layer.GetPreloadEntries())
                .Returns(new[]
                {
                    new EntryData(
                        true,
                        "dir5",
                        @"D:\dir1\dir2\src\data2\dir5",
                        @"D:\dir1\dir2\src\data2\",
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
            layerMoq.Setup(layer => layer.GetEntries(@"D:\dir1\dir2\src\data2\dir5"))
                .Returns(new[]
                {
                    new EntryData(
                        true,
                        "file1",
                        @"D:\dir1\dir2\src\data2\dir5\file1",
                        @"D:\dir1\dir2\src\data2\dir5",
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

            layerMoq.Setup(layer => layer.GetEntry(@"D:\dir1\dir2\src\data2\dir5\file1"))
                .Returns(entryMoq.Object)
                ;
            layerProviderMoq.Setup(provider => provider.CanCreateLayer(It.IsAny<Mod>())).Returns(true);
            layerProviderMoq.Setup(provider => provider.CreateLayer(It.IsAny<Mod>())).Returns(new[]{ layerMoq.Object});
            var target = new ModManager();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(@"
name: name
root_path: ""D:\\dir1\\dir2""
mod_directory: ""C:\\dir3\\dir4""
overlay_path: ""C:\\overlay""
base_path: ""C:\\base""
mods:
    - name: name
      mod_path: modA
      root_path: src
      description: aaaa
      links:
        - mod_path: data1
          target_path: data2
")); 
            await target.Load(stream, new []{layerProviderMoq.Object});
            FileInformation fileInformation;
            var result = target.Operation.GetFileInformation(@"D:\dir1\dir2\src\data2\dir5\file1", out fileInformation, new TestFileInfo());
            Assert.Equal(DokanResult.Success, result);
            Assert.Equal("file1", fileInformation.FileName);

        }
    }
}
