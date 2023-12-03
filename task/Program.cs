using Microsoft.EntityFrameworkCore;
using task.Data;
using Microsoft.AspNetCore.Identity;
using task.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AzureContext");

builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<TaskContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AzureContext")));

//builder.Services.AddDefaultIdentity<AccountUser>(options => options.SignIn.RequireConfirmedAccount = false)
 //   .AddRoles<IdentityRole>()
 //   .AddEntityFrameworkStores<TaskContext>();

builder.Services.AddDefaultIdentity<AccountUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<TaskContext>();builder.Services.AddDbContext<TaskContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint( "/api/swagger.json", "My API");
});
app.UseRouting();
app.UseAuthentication();
app.MapRazorPages();
app.UseAuthorization();

app.MapControllerRoute(
    name: "comment",
    pattern: "Comment/{action}/{id?}",
    defaults: new { controller = "Comment", action = "Index" });
    
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
    );

app.Run();    
              
    