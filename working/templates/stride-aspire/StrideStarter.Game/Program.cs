using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stride.Engine;
using StrideStarter.GameClient;
using StrideStarter.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();

// The hosted service owns disposal on the thread running the Stride window.
builder.Services.AddSingleton(new Game());
builder.Services.AddHostedService<GameLoop>();

using var host = builder.Build();
await host.RunAsync();
