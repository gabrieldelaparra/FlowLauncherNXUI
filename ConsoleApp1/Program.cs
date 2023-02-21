
var count = 0;
ShowParentWindow(args);

void ShowChildDialog(Window parent)
{
    Window Build()
        => Window(out var window)
            .Title("Child Dialog").Width(400).Height(300)
            .Content(
                StackPanel()
                    .Children(
                        Button(out var incrementCountButton)
                            .Content("Welcome to Avalonia, please click me!"),
                        TextBox(out var titleTextBox)
                            .Text("NXUI"),
                        TextBox()
                            .Text(window.BindTitle()),
                        Label()
                            .Content(
                                incrementCountButton.ObserveOnClick().Select(_ => ++count).Select(x => $"You clicked {x} times."))))
            .Title(titleTextBox.ObserveText().Select(x => x?.ToUpper()));

    //Depending on the case:
    Build().Show(parent);
    Build().ShowDialog(parent);
}

void ShowParentWindow(string[] strings)
{
    Window Build() =>
        Window(out var window)
            .Title("Parent Window").Width(400).Height(300)
            .Content(
                StackPanel()
                    .Children(
                        Button(out var button2)
                            .Content("Welcome to Avalonia, please click me!"),
                        Label()
                            .Content(button2.ObserveOnClick().Do(_ => ShowChildDialog(window)))
                    )
                );

    AppBuilder.Configure<Application>()
        .UsePlatformDetect()
        .UseFluentTheme()
        .WithApplicationName("NXUI")
        .StartWithClassicDesktopLifetime(Build, strings);
}
