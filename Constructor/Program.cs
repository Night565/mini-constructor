using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSession();
builder.Services.AddScoped<HomePageContentService>();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";
    
    if (path.StartsWith("/admin", StringComparison.OrdinalIgnoreCase) && 
        !path.StartsWith("/admin/login", StringComparison.OrdinalIgnoreCase) && 
        context.Session.GetInt32("AdminUserId") is null)
    {
        context.Response.Redirect("/admin/login");
        return;
    }

    await next();
});

app.MapRazorPages();

app.Run();
