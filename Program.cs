using dotenv.net;
using EncryptedDbAtRest.Components;
using EncryptedDbAtRest.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();
Env.Init();

// Add services to the container.
builder.Services.AddDbContext<EncryptedDbAtRest.Server.DbContext>(options =>
    {
        options.UseNpgsql(Env.DbString);
    });
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("api/get", (InstanceAccess.Get));
app.MapGet("api/create", InstanceAccess.Create);
app.MapGet("api/encr", InstanceAccess.TestDecrypt);

app.Run();