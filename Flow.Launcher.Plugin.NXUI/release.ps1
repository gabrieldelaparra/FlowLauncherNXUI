dotnet publish Flow.Launcher.Plugin.NXUI.csproj -c Release -r win-x64 -o publish
# ls
# Compress-Archive -LiteralPath publish -DestinationPath bin/NXUI.zip -Force