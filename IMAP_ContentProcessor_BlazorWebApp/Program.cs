using Google.Apis.Gmail.v1;
using IMAP_ContentProcessor_BlazorWebApp.Components;
using IMAP_ContentProcessor_BlazorWebApp.Infrastructure;
using IMAP_ContentProcessor_BlazorWebApp.Domain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = builder.Configuration["AuthScheme"];
    options.DefaultChallengeScheme = Google.Apis.Auth.AspNetCore3.GoogleOpenIdConnectDefaults.AuthenticationScheme;
})
    .AddCookie(builder.Configuration["AuthScheme"]!)
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
        options.SignInScheme = builder.Configuration["AuthScheme"];
        options.AccessType = "offline";
        options.AccessDeniedPath = "/";
        options.Scope.Add(GmailService.Scope.GmailCompose);
        options.Scope.Add(GmailService.Scope.GmailModify);
        options.Scope.Add(GmailService.Scope.GmailReadonly);
        options.SaveTokens = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

builder.Services.AddInfrastucture(builder.Configuration);
builder.Services.AddDomain();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication()
    .UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();