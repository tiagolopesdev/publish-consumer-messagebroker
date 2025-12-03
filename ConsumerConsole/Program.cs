// See https://aka.ms/new-console-template for more information
using ConsumerConsole;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Hello, World!");

IHost host = Host.CreateDefaultBuilder(args).ConfigureServices(services =>
{
    services.AddHostedService<ConsumerListening>();
}).Build();

await host.RunAsync();
