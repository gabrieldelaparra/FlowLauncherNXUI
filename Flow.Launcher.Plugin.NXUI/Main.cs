using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Flow.Launcher.Plugin;

namespace Flow.Launcher.Plugin.NXUI
{
    public class NXUI : IPlugin
    {
        private PluginInitContext _context;
        private string _executionPath;
        private AppBuilder _appBuilder;
        private int _count;

        public NXUI()
        {
            Log("Loading Plugin. Constructor");
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private const string LogPath = "C:\\Dev\\flowNXUI.log";

        public void Log(string message)
        {
            //To avoid logging, uncomment next line.
            File.AppendAllLines(LogPath, new[] { message });
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var fullName = args.Name;
            Log($"CurrentDomain_AssemblyResolve:{args.Name}");
            var directoryInfo = new DirectoryInfo(_executionPath);
            var dlls = directoryInfo.EnumerateFiles("*.dll", SearchOption.TopDirectoryOnly);
            var assemblies = dlls.Select(x => Assembly.LoadFrom(x.FullName));
            var assembly = assemblies.FirstOrDefault(x => x.FullName.Equals(fullName));
            Log($"Resolve:{assembly?.FullName ?? string.Empty}");
            return assembly;
        }

        public void Init(PluginInitContext context)
        {
            Log("Loading Plugin. Init");
            if (context == null)
            {
                Log("Exception: Null Context");
                return;
            }
            _context = context;

            _executionPath = new FileInfo(_context.CurrentPluginMetadata.ExecuteFilePath).Directory?.FullName ?? string.Empty;
            if (string.IsNullOrWhiteSpace(_executionPath))
                return;
            Log($"ExecutionPath: {_executionPath}");
        }

        public List<Result> Query(Query query)
        {
            Log("NXUI: Query");
            return new List<Result>()
            {
                new Result()
                {
                    Title = "Show NXUI",
                    Action = context =>
                    {
                        try
                        {
                            ShowDialog();
                        }
                        catch (Exception ex)
                        {
                            Log(ex.Message);
                        }
                        Log("Return");
                        Log($"Count: {_count}");
                        return true;
                    }
                }
            };
        }

        private void ShowDialog()
        {
            Window Build() => Window(out var window)
                .Title("NXUI").Width(400).Height(300)
                .Content(
                    StackPanel()
                        .Children(
                            Button(out var button)
                                .Content("Welcome to Avalonia, please click me!"),
                            Button(out var button2)
                                .Content("Welcome to Avalonia, please click me!"),
                            TextBox(out var tb1)
                                .Text("NXUI"),
                            TextBox()
                                .Text(window.BindTitle()),
                            Label()
                                .Content(button.ObserveOnClick().Select(_ => ++_count)
                                    .Select(x => $"You clicked {x} times.")),
                            Label()
                                .Content(button2.ObserveOnClick().Select(_ => ShowDialog()))
                            ))

                .Title(tb1.ObserveText().Select(x => x?.ToUpper()));

            Log("Configure");
            _appBuilder = AppBuilder.Configure<Application>()
                .UsePlatformDetect()
                .UseFluentTheme()
                .WithApplicationName("NXUI");

            Log("Start");
            _appBuilder.StartWithClassicDesktopLifetime(Build, Array.Empty<string>());
            Log("Closed");
        }
    }
}