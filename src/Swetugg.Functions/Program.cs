using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swetugg.Functions.Image;
using Swetugg.Shared.Helpers;

var builder = new HostBuilder();

builder.ConfigureServices(services =>
{
    services.AddTransient<IImageHelper, ImageHelper>();
});

var host = builder.ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();
