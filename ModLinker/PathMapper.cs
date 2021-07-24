using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLinker
{
    public class PathMapper
    {
        public string SourcePath { get; }
        public string TargetPath { get; }

        public PathMapper(string sourcePath, string targetPath)
        {
            SourcePath = sourcePath;
            TargetPath = targetPath;
        }

        public string GetMappedPath(string path)
        {
            if (!path.StartsWith(SourcePath))
            {
                throw new ArgumentException("path is most subset of root path.", path);
            }

            return Path.Combine(TargetPath, Path.GetRelativePath(SourcePath, path));
        }

        public string GetSourcePath(string path)
        {
            if (!path.StartsWith(TargetPath))
            {
                throw new ArgumentException("path is most subset of prefix path.", path);
            }

            return Path.Combine(SourcePath, Path.GetRelativePath(TargetPath, path));
        }

        public IEnumerable<string> GetPathFragmentsFromTargetPath(string path)
        {
            var fragments = Path.GetRelativePath(TargetPath, path)
                    .Split(Path.DirectorySeparatorChar)
                ;
            var result = new List<string>(fragments.Length);
            var temp = TargetPath;
            foreach (var fragment in fragments)
            {
                result.Add(temp = Path.Combine(temp, fragment));
            }

            return result;
        }

    }
}
