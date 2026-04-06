using QuestPDF.Infrastructure;
using StudentReportService.Services;

// QuestPDF community licence (free for open-source / non-commercial projects)
QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────────

builder.Services.AddControllers();

var nodeApiBaseUrl = builder.Configuration["NodeApi:BaseUrl"]
    ?? throw new InvalidOperationException("NodeApi:BaseUrl is required in appsettings.");

// Named HttpClient – base address points to the Node.js backend.
// Handlers are pooled so we can safely call CreateClient from a singleton.
builder.Services.AddHttpClient("NodeApi", client =>
{
    client.BaseAddress = new Uri(nodeApiBaseUrl);
});

// Singleton so that the authenticated session (cookies) is shared across requests.
builder.Services.AddSingleton<INodeApiClient, NodeApiClient>();
builder.Services.AddSingleton<IPdfReportService, PdfReportService>();

// ── Pipeline ──────────────────────────────────────────────────────────────────

var app = builder.Build();

app.MapControllers();

app.Run();
