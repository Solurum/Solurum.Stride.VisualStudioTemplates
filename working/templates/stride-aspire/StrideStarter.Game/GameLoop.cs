using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Stride.CommunityToolkit.Engine;
using Stride.CommunityToolkit.Rendering.ProceduralModels;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Rendering.Materials;
using Stride.Rendering.Materials.ComputeColors;

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
                using var meter = new Meter("Stride.Game");
                using var activities = new ActivitySource("Stride.Game");
                var frames = meter.CreateCounter<long>("stride.game.frames", "{frame}");
                var frameTime = meter.CreateHistogram<double>("stride.game.frame.duration", "ms");
                Texture? texture = null;
                Entity? cube = null;
                float angle = 0;

                try
                {
                    game.Run(
                        start: scene =>
                        {
                            using var sceneLoad = activities.StartActivity("Build cube scene");
                            game.AddGraphicsCompositor();
                            game.Add3DCamera("Main Camera", new Vector3(0, 0, 5), Vector3.Zero);
                            var light = game.AddDirectionalLight(intensity: 3);
                            light.Transform.Rotation = Quaternion.RotationYawPitchRoll(0.45f, -0.35f, 0);
                            cube = game.Create3DPrimitive(PrimitiveModelType.Cube);
                            texture = CreateCheckerTexture(game.GraphicsDevice);
                            cube.Get<ModelComponent>().Materials[0] = CreateCubeMaterial(game.GraphicsDevice, texture);
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

                            frames.Add(1);
                            frameTime.Record(time.Elapsed.TotalMilliseconds);

                            if (cube is not null)
                            {
                                angle += (float)time.Elapsed.TotalSeconds;
                                cube.Transform.Rotation = Quaternion.RotationY(angle);
                            }
                        });
                }
                finally
                {
                    texture?.Dispose();
                }
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

    private static Texture CreateCheckerTexture(GraphicsDevice device)
    {
        const int size = 64;
        const int tileSize = 16;
        var cyan = new Color(35, 210, 230);
        var navy = new Color(20, 45, 95);
        var pixels = new Color[size * size];

        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                pixels[y * size + x] = ((x / tileSize + y / tileSize) % 2 == 0) ? cyan : navy;
            }
        }

        return Texture.New2D(device, size, size, PixelFormat.R8G8B8A8_UNorm_SRgb,
            pixels, TextureFlags.ShaderResource, GraphicsResourceUsage.Immutable);
    }

    private static Material CreateCubeMaterial(GraphicsDevice device, Texture texture)
    {
        return Material.New(device, new MaterialDescriptor
        {
            Attributes =
            {
                Diffuse = new MaterialDiffuseMapFeature(new ComputeTextureColor(texture)),
                DiffuseModel = new MaterialDiffuseLambertModelFeature(),
                Emissive = new MaterialEmissiveMapFeature(new ComputeTextureColor(texture))
                {
                    Intensity = new ComputeFloat(0.25f)
                }
            }
        });
    }
}
