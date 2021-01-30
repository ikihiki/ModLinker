using System;
using DokanNet;
using Microsoft.DotNet.Interactive.Formatting;

namespace ModLinker.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var target = new Target
            {
                RootPath = @"C:\Users\ibuki\source\repos\ModLinker\ModLinker.Cli\TestDir\Root",
                BasePath = @"C:\Users\ibuki\source\repos\ModLinker\ModLinker.Cli\TestDir\Base",
                ModDirectory = @"C:\Users\ibuki\source\repos\ModLinker\ModLinker.Cli\TestDir\Mods",
                Mods = new[]
                {
                    new Mod
                    {
                        Description = "Mod1", ModPath = @"C:\Users\ibuki\source\repos\ModLinker\ModLinker.Cli\TestDir\Mods\Mod1", Order = 1, Name = "Mod1", RootPath = "Layers",
                        Links = new[] {new Link {ModPath = "Layer1", TargetPath = "Layer"}}
                    }
                },
                OverlayPath = @"C:\Users\ibuki\source\repos\ModLinker\ModLinker.Cli\TestDir\Overlay"
            };
            var manager = new LayerService(target, new[] { new DirectoryLayerProvider() });
            if (args.Length < 2)
            {


                Console.CancelKeyPress += (sender, eventArgs) => manager.Dispose();
                manager.Notify += () => ShowFiles(manager);
                ShowFiles(manager);
                while (Console.Read() != 'q') ;
                manager.Dispose();
            }
            else
            {
                var op = new ModLinkerOperation(manager);
                op.Mount(target.RootPath);
            }
        }


        static void ShowFiles(LayerService service)
        {
            Console.WriteLine("\\");
            var children = service.GetEntries("\\");
            foreach (var child in children)
            {
                ShowFiles(child, service);
            }
        }

        static void ShowFiles(Entry entry, LayerService service)
        {
            Console.WriteLine(entry.ToDisplayString());
            var children = service.GetEntries(entry.Path);
            foreach (var child in children)
            {
                ShowFiles(child, service);
            }
        }
    }

}
