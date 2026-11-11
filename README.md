# Solurum.Stride.VisualStudioTemplates

A Visual Studio and .NET CLI solution template for a code-only Stride game with Aspire. The sample opens a desktop window with a spinning checker-textured cube. It uses a procedural mesh and Stride Community Toolkit scene setup.

## Install and create a solution

Use Windows, the .NET 10 SDK, and Visual Studio with .NET 10 support if you want to launch from the IDE. From a clone of this repository:

```powershell
dotnet pack working/templatepack.csproj -c Release
dotnet new install ./working/bin/Release/Solurum.Stride.VisualStudioTemplates.0.2.0.nupkg
dotnet new solurum-stride-aspire -n MyGame -o MyGame
dotnet build ./MyGame/MyGame.sln
```

Use a C# identifier or a dotted name for the solution. In Visual Studio, look for **Stride Code-Only Game with Aspire** under **Create a new project** after installing the package.

Open the generated solution and select **MyGame.AppHost** as the startup project. Use its **https** launch profile. If the development certificate is missing or untrusted, run `dotnet dev-certs https --trust`. You can also run the AppHost with `dotnet run --project ./MyGame/MyGame.AppHost` or start the game directly with `dotnet run --project ./MyGame/MyGame.Game`.

The AppHost starts the game as a local desktop process and opens the Aspire dashboard when launched from Visual Studio. The game uses the .NET Generic Host and its ServiceDefaults project to export telemetry when Aspire supplies an OTLP endpoint. The hosted game loop builds the scene in C# and disposes Stride on the same thread that ran it. There are no container or broker dependencies.

[See the live-run Aspire screenshots and telemetry walkthrough](working/templates/stride-aspire/README.md).

The scene setup follows the [Stride Community Toolkit code-only examples](https://stride3d.github.io/stride-community-toolkit/manual/code-only/create-file-based-app.html).


The starter uses the Stride toolkit's transitive ImageSharp 3.1.12 dependency, which currently produces NuGet audit warnings.
