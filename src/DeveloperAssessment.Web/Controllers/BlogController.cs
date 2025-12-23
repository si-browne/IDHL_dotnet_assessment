using DeveloperAssessment.Common.Models.Blog;
using DeveloperAssessment.Common.ViewModels;
using DeveloperAssessment.Services.Contracts;
using DeveloperAssessment.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DeveloperAssessment.Web.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly ILogger<BlogController> _logger;

        public BlogController(IBlogService blogService, ILogger<BlogController> logger)
        {
            _blogService = blogService;
            _logger = logger;
        }

        // GET /blog
        public async Task<IActionResult> Index()
        {
            var posts = await _blogService.GetPostsAsync();
            return View(posts);
        }

        // GET /blog/{id}
        public async Task<IActionResult> Post(int id)
        {
            var post = await _blogService.GetPostByIdAsync(id);
            if (post is null) return NotFound();

            return View("Blog", new BlogPostPageViewModel
            {
                Post = post,
                NewComment = new CommentInputModel()
            });
        }

        // POST /blog/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post(int id, BlogPostPageViewModel model)
        {
            var post = await _blogService.GetPostByIdAsync(id);
            if (post is null) return NotFound();

            model.Post = post;

            if (!ModelState.IsValid)
                return View("Blog", model);

            await _blogService.AddCommentAsync(id, new Common.Models.Blog.CommentItem
            {
                Name = model.NewComment.Name,
                EmailAddress = model.NewComment.EmailAddress,
                Message = model.NewComment.Message,
                Date = DateTime.UtcNow
            });

            return RedirectToAction(nameof(Post), new { id });
        }
    }
}
