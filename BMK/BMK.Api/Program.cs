using BMK.Api;

var builder = WebApplication.CreateBuilder(args);

// Load the appropriate appsettings file based on the environment
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
builder.Configuration.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

await builder.ConfigureServices().ConfigureAsync();
