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
        private readonly ICommentFileService _fileStore;

        public BlogController(IBlogService blogService, ILogger<BlogController> logger, ICommentFileService fileStore)
        {
            _blogService = blogService;
            _logger = logger;
            _fileStore = fileStore;
        }

        // GET /blog?page=1
        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 6;

            _logger.LogInformation("Loading blog index page {Page}", page);

            var allPosts = await _blogService.GetPostsAsync();
            var totalCount = allPosts.Count;

            page = Math.Max(page, 1);

            var posts = allPosts.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            _logger.LogInformation("Returning {PostCount} posts for page {Page} (Total posts: {TotalCount})", posts.Count, page, totalCount);

            var vm = new BlogIndexViewModel
            {
                Posts = posts,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return View(vm);
        }

        // GET /blog/{id}
        [HttpGet]
        public async Task<IActionResult> Post(int id)
        {
            _logger.LogInformation("Loading blog post {PostId}", id);

            var post = await _blogService.GetPostByIdAsync(id);
            if (post is null) 
            {
                _logger.LogWarning("Blog post {PostId} not found", id);
                return NotFound();
            }
            
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
            _logger.LogInformation("Attempting to add comment to post {PostId}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid comment model for post {PostId}. Errors: {ErrorCount}", id, ModelState.ErrorCount);

                var post = await _blogService.GetPostByIdAsync(id);

                if (post is null) 
                {
                    _logger.LogWarning("Blog post {PostId} not found during comment validation", id);
                    return NotFound();
                }

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

            var attachments = new List<CommentAttachmentItem>();

            foreach (var file in input.Files ?? new List<IFormFile>())
            {
                if (file is null || file.Length == 0) continue;

                try
                {
                    _logger.LogInformation("Saving attachment {FileName} for post {PostId}", file.FileName, id);
                    attachments.Add(await _fileStore.SaveAsync(file));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save attachment {FileName} for post {PostId}", file.FileName, id);
                    // here, iff an upload fails, show a friendly error and re-render
                    ModelState.AddModelError(string.Empty, $"Attachment '{file.FileName}' failed: {ex.Message}");

                    var post = await _blogService.GetPostByIdAsync(id);
                    if (post is null) 
                    {
                        _logger.LogWarning("Blog post {PostId} not found after attachment failure", id);
                        return NotFound();
                    }

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
            }

            await _blogService.AddCommentAsync(id, new CommentItem
            {
                Name = input.Name,
                EmailAddress = input.EmailAddress,
                Message = input.Message,
                Date = DateTime.UtcNow,
                Attachments = attachments
            });

            _logger.LogInformation("Successfully added comment to post {PostId} with {AttachmentCount} attachments", id, attachments.Count);

            return RedirectToAction(nameof(Post), new { id });
        }

        [HttpPost("blog/{id:int}/comments/{commentId:int}/reply")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(int id, int commentId, [Bind(Prefix = "NewReply")] AddReplyRequest input)
        {
            _logger.LogInformation("Attempting to add reply to comment {CommentId} on post {PostId}", commentId, id);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid reply model for post {PostId}, comment {CommentId}", id, commentId);
                var post = await _blogService.GetPostByIdAsync(id);

                if (post is null)
                {
                    _logger.LogWarning("Blog post {PostId} not found during reply validation", id);
                    return NotFound();
                }

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

            _logger.LogInformation("Successfully added reply to comment {CommentId} on post {PostId}", commentId, id);

            return RedirectToAction(nameof(Post), new { id });
        }
    }
}
