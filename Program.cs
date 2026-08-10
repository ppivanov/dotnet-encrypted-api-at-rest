using dotenv.net;
using EncryptedDbAtRest.Components;
using EncryptedDbAtRest.Server;
using EncryptedDbAtRest.Server.Encryption;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();
Env.Initialize();
Thread.Sleep(1000); // wait 1 second before attempting to initialize the algorithms
SymmetricEncryption.Initialize();

// Add services to the container.
builder.Services.AddDbContext<EncryptedDbAtRest.Server.DbContext>(options =>
    {
        options.UseNpgsql(Env.DbString);
    });
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!Env.IsDevelopment)
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.UseTimeTracking();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

//app.UseCookieSessions();
app.MapGet("api/get", (InstanceAccess.Get));
app.MapGet("api/create", InstanceAccess.Create);

app.Run();