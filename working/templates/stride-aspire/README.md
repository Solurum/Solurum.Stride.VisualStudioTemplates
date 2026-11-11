# StrideStarter

Open StrideStarter.sln in Visual Studio on Windows with the .NET 10 SDK. Select StrideStarter.AppHost as the startup project and use the https profile, or run:

```powershell
dotnet run --project ./StrideStarter.AppHost
```

The game opens a window with a spinning checker-textured cube. The scene, material, and texture are created in C#. You can also launch StrideStarter.Game directly.

## Aspire dashboard

Select the game resource on the **Metrics** page. Under **Stride.Game**, select `stride.game.frame.duration` for Stride's reported elapsed time per update in milliseconds, or `stride.game.frames` for the update counter. **System.Runtime** includes working set, GC, and thread pool metrics. Allow up to a minute for the first metrics export.

In **Traces**, look for **Build cube scene** to see how long scene initialization took. In **Structured logs**, look for **Stride cube scene loaded**. The log is emitted inside the startup span so the dashboard can link the two.

[Frame timing screenshot](docs/aspire-metrics.png) · [Scene startup trace screenshot](docs/aspire-traces.png) · [Trace-linked log screenshot](docs/aspire-logs.png)
