using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using DokanNet;
using LiteDB;

namespace ModLinker
{

    public class EntryDataDto
    {
        private ObjectId Id { get; set; }
        public bool IsLazy { get; set; }
        public string ParentDirectory { get; set; }
        public string Path { get; set; }
        public bool IsDirectory { get; set; }
        public string LayerId { get; set; }
        public DateTime? CreationTime { get; set; }
        public DateTime? LastAccessTime { get; set; }
        public DateTime? LastWriteTime { get; set; }
        public long Length { get; set; }
        public bool Writable { get; set; }
        public uint Priority { get; set; }

        public EntryDataDto(){}

        public EntryDataDto(EntryData entryData)
        {
            IsLazy = entryData.IsLazy;
            ParentDirectory = entryData.ParentDirectory;
            Path = entryData.Path;
            IsDirectory = entryData.IsDirectory;
            LayerId = entryData.Layer.Id;
            CreationTime = entryData.CreationTime;
            LastAccessTime = entryData.LastAccessTime;
            LastWriteTime = entryData.LastWriteTime;
            Length = entryData.Length;
            Writable = entryData.Writable;
            Priority = entryData.Priority;
        }

        public static IEnumerable<EntryDataDto> Convert(IEnumerable<EntryData> source)
        {
            return source.Select(entry => new EntryDataDto(entry));
        }
    }

    public class LayerService
    {
        private readonly Dictionary<string, ILayer> layers;
        private readonly ILiteDatabase database;
        private readonly ILiteCollection<EntryDataDto> collection;
        private readonly PathMapper mapper;

        public LayerService(IEnumerable<ILayer> layers, string targetRoot)
        {
            this.layers = layers.ToDictionary(layer => layer.Id);
            mapper = new PathMapper("", targetRoot);
            database = new LiteDatabase(@"mydata.db");
            collection = database.GetCollection<EntryDataDto>("entry");
            collection.DeleteAll();
        }

        public async ValueTask PreLoad()
        {
            foreach (var layer in layers.Values)
            {
                var entries = EntryDataDto.Convert(layer.GetPreloadEntries());
                collection.Upsert(entries);
            }
        }

        public IEnumerable<IEntry> GetEntries(string path)
        {
            var frags = mapper.GetPathFragmentsFromTargetPath(path);

            foreach (var frag in frags)
            {
                var e = FindEntry(frag);
                ;
                if (e.IsLazy)
                {
                    var layer = layers[e.LayerId];
                    var es = layer.GetEntries(frag);
                    collection.Upsert(EntryDataDto.Convert(es));
                }
            }

            var result = collection.Find(dto => dto.ParentDirectory == path)
                    .GroupBy(dto => dto.Path)
                    .Select(g => g.OrderBy(dto => dto.Priority).FirstOrDefault())
                    .Where(dto => dto != null)
                    .Select(dto => layers[dto.LayerId].GetEntry(dto.Path))
                    .ToArray()
                ;

            return result;
        }

        public IEntry GetEntry(string fileName)
        {
            var frags = mapper.GetPathFragmentsFromTargetPath(fileName);

            foreach (var frag in frags.SkipLast(1))
            {
                var e = FindEntry(frag);
                ;
                if (e?.IsLazy == true)
                {
                    var layer = layers[e.LayerId];
                    var es = layer.GetEntries(frag);
                    collection.Upsert(EntryDataDto.Convert(es));
                }
            }

            var result = collection.Find(dto => dto.Path == fileName)
                    .OrderBy(dto => dto.Priority)
                    .Select(dto => layers[dto.LayerId].GetEntry(dto.Path))
                    .FirstOrDefault()
                ;

            return result;
        }

        internal IFileHandle UpdateWritable(IFileHandle handle)
        {
            throw new NotImplementedException();
        }

        public NtStatus DeleteFile(string fileName)
        {
            throw new NotImplementedException();
        }

        internal NtStatus DeleteDirectory(string fileName)
        {
            throw new NotImplementedException();
        }

        internal NtStatus MoveFile(string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        public IEntry UpdateWritable(IEntry handle)
        {
            throw new NotImplementedException();
        }


        private EntryDataDto FindEntry(string path)
        {
            return collection.Find(dto => dto.Path == path)
                    .OrderBy(dto => dto.Priority)
                    .FirstOrDefault()
                ;
        }
    }
}
