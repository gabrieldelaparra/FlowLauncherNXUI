using System.IO;
using System.Linq;
using System.Reflection;

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
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }


        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var fullName = args.Name;
            var directoryInfo = new DirectoryInfo(_executionPath);
            var dlls = directoryInfo.EnumerateFiles("*.dll", SearchOption.TopDirectoryOnly);
            var assemblies = dlls.Select(x => Assembly.LoadFrom(x.FullName));
            var assembly = assemblies.FirstOrDefault(x => x.FullName.Equals(fullName));
            return assembly;
        }

        public void Init(PluginInitContext context)
        {
            if (context == null)
            {
                return;
            }
            _context = context;

            _executionPath = new FileInfo(_context.CurrentPluginMetadata.ExecuteFilePath).Directory?.FullName ?? string.Empty;
            if (string.IsNullOrWhiteSpace(_executionPath))
                return;
        }

        public List<Result> Query(Query query)
        {
            return new List<Result>()
            {
                new Result()
                {
                    Title = "Show NXUI",
                    Action = context =>
                    {
                            ShowDialog();
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
                            TextBox(out var tb1)
                                .Text("NXUI"),
                            TextBox()
                                .Text(window.BindTitle()),
                            Label()
                                .Content(button.ObserveOnClick().Select(_ => ++_count)
                                    .Select(x => $"You clicked {x} times."))
                            ))

                .Title(tb1.ObserveText().Select(x => x?.ToUpper()));

            _appBuilder = AppBuilder.Configure<Application>()
                .UsePlatformDetect()
                .UseFluentTheme()
                .WithApplicationName("NXUI");

            _appBuilder.StartWithClassicDesktopLifetime(Build, Array.Empty<string>());
        }
    }
}