using DeveloperAssessment.Common.Models.Blog;
using DeveloperAssessment.DAL;
using DeveloperAssessment.DAL.Contracts;
using DeveloperAssessment.Services.Contracts;
using DeveloperAssessment.Services.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache(); // middleware caching

builder.Services.AddScoped<IRepository<BlogPostDocument>>(sp =>
{
    var env = sp.GetRequiredService<IHostEnvironment>();
    // relative to ContentRootPath inside repository
    return new GenericRepository<BlogPostDocument>(env, Path.Combine("App_Data", "Blog-Posts.json"));
});

// the service layer
builder.Services.AddScoped<IBlogService, BlogService>();

builder.Services.AddScoped<ICommentFileService, CommentFileService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// the blog routes (exercise 1,2,3 and 5)
app.MapControllerRoute(
    name: "blog-post",
    pattern: "blog/{id:int}",
    defaults: new { controller = "Blog", action = "Post" });

app.MapControllerRoute(
    name: "blog-index",
    pattern: "blog",
    defaults: new { controller = "Blog", action = "Index" });

// exercise 4
app.MapControllerRoute(
    name: "blog-reply",
    pattern: "blog/{id:int}/comments/{commentId:int}/reply",
    defaults: new { controller = "Blog", action = "Reply" });

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// simulation of unit tests, see XML comment
if (app.Environment.IsDevelopment())
{
    await DeveloperAssessment.Services.Tests.BlogServiceTests.RunAll();
}

app.Run();