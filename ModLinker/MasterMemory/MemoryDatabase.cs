// <auto-generated />
#pragma warning disable CS0105
using MasterMemory.Validation;
using MasterMemory;
using MessagePack;
using ModLinker;
using System.Collections.Generic;
using System;
using ModLinker.Tables;

namespace ModLinker
{
   public sealed class MemoryDatabase : MemoryDatabaseBase
   {
        public DirectoryTable DirectoryTable { get; private set; }
        public FileTable FileTable { get; private set; }
        public ModTable ModTable { get; private set; }
        public TargetTable TargetTable { get; private set; }

        public MemoryDatabase(
            DirectoryTable DirectoryTable,
            FileTable FileTable,
            ModTable ModTable,
            TargetTable TargetTable
        )
        {
            this.DirectoryTable = DirectoryTable;
            this.FileTable = FileTable;
            this.ModTable = ModTable;
            this.TargetTable = TargetTable;
        }

        public MemoryDatabase(byte[] databaseBinary, bool internString = true, MessagePack.IFormatterResolver formatterResolver = null)
            : base(databaseBinary, internString, formatterResolver)
        {
        }

        protected override void Init(Dictionary<string, (int offset, int count)> header, System.ReadOnlyMemory<byte> databaseBinary, MessagePack.MessagePackSerializerOptions options)
        {
            this.DirectoryTable = ExtractTableData<Directory, DirectoryTable>(header, databaseBinary, options, xs => new DirectoryTable(xs));
            this.FileTable = ExtractTableData<File, FileTable>(header, databaseBinary, options, xs => new FileTable(xs));
            this.ModTable = ExtractTableData<Mod, ModTable>(header, databaseBinary, options, xs => new ModTable(xs));
            this.TargetTable = ExtractTableData<Target, TargetTable>(header, databaseBinary, options, xs => new TargetTable(xs));
        }

        public ImmutableBuilder ToImmutableBuilder()
        {
            return new ImmutableBuilder(this);
        }

        public DatabaseBuilder ToDatabaseBuilder()
        {
            var builder = new DatabaseBuilder();
            builder.Append(this.DirectoryTable.GetRawDataUnsafe());
            builder.Append(this.FileTable.GetRawDataUnsafe());
            builder.Append(this.ModTable.GetRawDataUnsafe());
            builder.Append(this.TargetTable.GetRawDataUnsafe());
            return builder;
        }

        public DatabaseBuilder ToDatabaseBuilder(MessagePack.IFormatterResolver resolver)
        {
            var builder = new DatabaseBuilder(resolver);
            builder.Append(this.DirectoryTable.GetRawDataUnsafe());
            builder.Append(this.FileTable.GetRawDataUnsafe());
            builder.Append(this.ModTable.GetRawDataUnsafe());
            builder.Append(this.TargetTable.GetRawDataUnsafe());
            return builder;
        }

        public ValidateResult Validate()
        {
            var result = new ValidateResult();
            var database = new ValidationDatabase(new object[]
            {
                DirectoryTable,
                FileTable,
                ModTable,
                TargetTable,
            });

            ((ITableUniqueValidate)DirectoryTable).ValidateUnique(result);
            ValidateTable(DirectoryTable.All, database, "Path", DirectoryTable.PrimaryKeySelector, result);
            ((ITableUniqueValidate)FileTable).ValidateUnique(result);
            ValidateTable(FileTable.All, database, "Path", FileTable.PrimaryKeySelector, result);
            ((ITableUniqueValidate)ModTable).ValidateUnique(result);
            ValidateTable(ModTable.All, database, "Id", ModTable.PrimaryKeySelector, result);
            ((ITableUniqueValidate)TargetTable).ValidateUnique(result);
            ValidateTable(TargetTable.All, database, "Id", TargetTable.PrimaryKeySelector, result);

            return result;
        }

        static MasterMemory.Meta.MetaDatabase metaTable;

        public static object GetTable(MemoryDatabase db, string tableName)
        {
            switch (tableName)
            {
                case "directories":
                    return db.DirectoryTable;
                case "files":
                    return db.FileTable;
                case "mods":
                    return db.ModTable;
                case "target":
                    return db.TargetTable;
                
                default:
                    return null;
            }
        }

        public static MasterMemory.Meta.MetaDatabase GetMetaDatabase()
        {
            if (metaTable != null) return metaTable;

            var dict = new Dictionary<string, MasterMemory.Meta.MetaTable>();
            dict.Add("directories", ModLinker.Tables.DirectoryTable.CreateMetaTable());
            dict.Add("files", ModLinker.Tables.FileTable.CreateMetaTable());
            dict.Add("mods", ModLinker.Tables.ModTable.CreateMetaTable());
            dict.Add("target", ModLinker.Tables.TargetTable.CreateMetaTable());

            metaTable = new MasterMemory.Meta.MetaDatabase(dict);
            return metaTable;
        }
    }
}