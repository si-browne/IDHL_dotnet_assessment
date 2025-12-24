using DeveloperAssessment.Common.Models.Blog;
using DeveloperAssessment.Common.Requests.Blog;
using DeveloperAssessment.Services.Contracts;
using DeveloperAssessment.Web.Models.Blog;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Post(int id, [Bind(Prefix = "NewComment")] AddCommentRequest input)
        {
            if (!ModelState.IsValid)
            {
                var post = await _blogService.GetPostByIdAsync(id);

                if (post is null) return NotFound();

                return View("Blog", new BlogPostPageViewModel
                {
                    Post = post,
                    NewComment = new CommentInputModel
                    {
                        Name = input.Name,
                        EmailAddress = input.EmailAddress,
                        Message = input.Message
                    },
                    NewReply = new ReplyInputModel()
                });
            }

            await _blogService.AddCommentAsync(id, new CommentItem
            {
                Name = input.Name,
                EmailAddress = input.EmailAddress,
                Message = input.Message,
                Date = DateTime.UtcNow
            });

            return RedirectToAction(nameof(Post), new { id });
        }

        [HttpPost("blog/{id:int}/comments/{commentId:int}/reply")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(int id, int commentId, [Bind(Prefix = "NewReply")] AddReplyRequest input)
        {
            if (!ModelState.IsValid)
            {
                var post = await _blogService.GetPostByIdAsync(id);

                if (post is null) return NotFound();

                return View("Blog", new BlogPostPageViewModel
                {
                    Post = post,
                    NewComment = new CommentInputModel(),
                    NewReply = new ReplyInputModel
                    {
                        Name = input.Name,
                        EmailAddress = input.EmailAddress,
                        Message = input.Message
                    }
                });
            }

            await _blogService.AddReplyAsync(id, commentId, new CommentReplyItem
            {
                Name = input.Name,
                EmailAddress = input.EmailAddress,
                Message = input.Message,
                Date = DateTime.UtcNow
            });

            return RedirectToAction(nameof(Post), new { id });
        }
    }
}
