using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using News_Aggregator.Data;
using News_Aggregator.EntityModel;
using News_Aggregator.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure EF Core with SQL Server and connection string from appsettings.json
builder.Services.AddDbContext<NewsContext>(x =>

{
    x.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<NewsContext>()
    .AddDefaultTokenProviders();

// Retrieve the API key from the appsettings.json or environment variables
var apiKey = builder.Configuration["NewsApiSettings:ApiKey"];

builder.Services.AddHttpClient<NewsService>((sp, client) =>
{
    client.BaseAddress = new Uri("https://newsapi.org/v2/");
    client.DefaultRequestHeaders.Add("User-Agent", "NewsAggregatorApp/3.0"); // Set your User-Agent header
    client.DefaultRequestHeaders.Add("x-api-key", apiKey); // Add API key in header
});


// Register EmailService and ArticleService
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<IArticleService, ArticleService>();

// Add Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Enforce HTTP Strict Transport Security (HSTS)
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Enable serving static files (like CSS, JS, etc.)
app.UseRouting(); // for routing to controllers
app.UseAuthentication(); // Added for authentication
app.UseAuthorization(); // Added for authorization

// Set up the default controller route (HomeController with Index action)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();