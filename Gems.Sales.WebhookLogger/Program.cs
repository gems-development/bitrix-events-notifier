using Serilog;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

if (app.Environment.IsDevelopment())
	app.MapOpenApi();

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.WriteTo.File("/var/log/gems/bitrix-notifier/webhooks.log")
	.CreateLogger();

app.MapPost("/webhooks", async (HttpRequest request) =>
	{
		request.EnableBuffering();

		using var reader = new StreamReader(request.Body, leaveOpen: true);
		var body = await reader.ReadToEndAsync();

		Log.Logger.Information("{Body}", body);
	});

await app.RunAsync();
