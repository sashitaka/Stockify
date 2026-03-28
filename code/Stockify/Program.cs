using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stockify.Data;
using Stockify.Models;
using Stockify.Services;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

// Blazor
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// HttpClient for Massive API
builder.Services.AddHttpClient<StockService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["MassiveApi:BaseUrl"]!);
    client.DefaultRequestHeaders.Add("x-api-key", builder.Configuration["MassiveApi:ApiKey"]);
});

// App services
builder.Services.AddScoped<PortfolioService>();
builder.Services.AddScoped<TransactionService>();
builder.Services.AddScoped<WatchlistService>();

var app = builder.Build();

// Seed roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var role in new[] { "ADMIN", "CLIENT" })
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
