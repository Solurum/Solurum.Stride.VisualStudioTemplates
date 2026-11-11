using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Stride.CommunityToolkit.Engine;
using Stride.CommunityToolkit.Rendering.ProceduralModels;
using Stride.Core.Mathematics;
using Stride.Engine;

namespace StrideStarter.GameClient;

internal sealed class GameLoop(
    Game game,
    IHostApplicationLifetime lifetime,
    ILogger<GameLoop> logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using (game)
            {
                Entity? cube = null;
                float angle = 0;

                game.Run(
                    start: scene =>
                    {
                        game.AddGraphicsCompositor();
                        game.Add3DCamera("Main Camera", new Vector3(0, 0, 5), Vector3.Zero);
                        game.AddDirectionalLight();
                        cube = game.Create3DPrimitive(PrimitiveModelType.Cube);
                        cube.Scene = scene;
                        logger.LogInformation("Stride cube scene loaded");
                    },
                    update: (_, time) =>
                    {
                        if (stoppingToken.IsCancellationRequested)
                        {
                            game.Exit();
                            return;
                        }

                        if (cube is not null)
                        {
                            angle += (float)time.Elapsed.TotalSeconds;
                            cube.Transform.Rotation = Quaternion.RotationY(angle);
                        }
                    });
            }
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, "Stride stopped unexpectedly");
            Environment.ExitCode = 1;
            throw;
        }
        finally
        {
            lifetime.StopApplication();
        }

        return Task.CompletedTask;
    }
}
