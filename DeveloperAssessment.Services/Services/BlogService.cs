using DeveloperAssessment.Common.Models.Blog;
using DeveloperAssessment.DAL.Contracts;
using DeveloperAssessment.Services.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace DeveloperAssessment.Services.Services
{
    // S.O.L.I.D -- I will place the bracketted acronym & description where I believe each is demonstrated.. like so.. (SRP)
    // Because ISP relates to interfaces - i placed my acronym and description in there
    // Liskov I found to be a challenge and is used in the base class of some of the models - i placed my acronym and description in ItemBase.cs as well as some of the generics employed

    // Single Responsibility Principle (SRP)
    // Open Closed Principle (OCP)
    // Liskov Subsititution Principle (LSP)
    // Interface Segregation Principle (ISP)
    // Dependency Inversion Principle (DIP)

    /// <summary>
    /// Blog Service performs operations on Blog data to be injected into controller.
    /// </summary>
    public class BlogService : IBlogService
    {
        private const string CacheKey = "blogposts:document";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5); // decided on 5 miniutes initially

        // (LSP) - Generic repositority substitutes types by its nature
        private readonly IRepository<BlogPostDocument> _repository;
        private readonly IMemoryCache _cache;

        public BlogService(IRepository<BlogPostDocument> repository, IMemoryCache cache)
        {
            // (DIP) - depends on abstractions (IRepository, IMemoryCache), not concrete implementations
            _repository = repository;
            _cache = cache;
        }

        public async Task<IReadOnlyList<BlogPostItem>> GetPostsAsync()
        {
            // (SRP) - this method has one job: return a single post by id
            var doc = await GetDocumentCachedAsync();
            return doc.BlogPosts.OrderByDescending(p => p.Date).ToList(); // with date ordering built in
        }

        public async Task<BlogPostItem?> GetPostByIdAsync(int id)
        {
            // (SRP) - this method has one job: return posts (sorted) for read scenarios
            var doc = await GetDocumentCachedAsync();
            return doc.BlogPosts.FirstOrDefault(p => p.Id == id);
        }

        public async Task AddCommentAsync(int postId, CommentItem comment)
        {
            // (SRP) - this method has one job: add a comment and persist it
            var doc = await _repository.GetAsync();
            var post = doc.BlogPosts.FirstOrDefault(p => p.Id == postId);

            if (post is null) 
            {
                throw new InvalidOperationException($"Blog post {postId} not found.");
            }
                
            // ID allocation (JSON currently omits comment ids)
            var nextId = post.Comments.Count == 0 ? 1 : post.Comments.Max(c => c.Id) + 1;
            comment.Id = nextId;

            post.Comments.Add(comment);

            await _repository.SaveAsync(doc);

            // code to invalidate cache so next request reflects persisted data
            _cache.Remove(CacheKey);
        }

        private async Task<BlogPostDocument> GetDocumentCachedAsync()
        {
            // (OCP) - caching behaviour can be changed/extended by swapping cache strategy (IMemoryCache) without changing calling code (public methods still call PRIVATE GetDocumentCachedAsync()).
            if (_cache.TryGetValue(CacheKey, out BlogPostDocument? cached) && cached is not null) 
            {
                return cached;
            }

            var doc = await _repository.GetAsync();

            _cache.Set(CacheKey, doc, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration
            });

            return doc;
        }

        public async Task AddReplyAsync(int postId, int commentId, CommentReplyItem reply)
        {
            var doc = await _repository.GetAsync();
            var post = doc.BlogPosts.FirstOrDefault(p => p.Id == postId);

            if (post is null)
            {
                throw new InvalidOperationException($"Blog post {postId} not found.");
            }

            var comment = post.Comments.FirstOrDefault(c => c.Id == commentId);

            if (comment is null) 
            {
                throw new InvalidOperationException($"Comment {commentId} not found.");
            }

            var nextReplyId = comment.Replies.Count == 0 ? 1 : comment.Replies.Max(r => r.Id) + 1;
            reply.Id = nextReplyId;
            comment.Replies.Add(reply);

            await _repository.SaveAsync(doc);
            _cache.Remove(CacheKey);
        }
    }
}
