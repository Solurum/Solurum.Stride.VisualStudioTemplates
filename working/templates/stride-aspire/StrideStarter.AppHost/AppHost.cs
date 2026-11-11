var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.StrideGame>("game");

builder.Build().Run();
