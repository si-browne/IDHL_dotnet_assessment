using DeveloperAssessment.Common.Models.Blog;
using DeveloperAssessment.DAL.Contracts;
using DeveloperAssessment.Services.Services;
using Microsoft.Extensions.Caching.Memory;

namespace DeveloperAssessment.Services.Tests
{
    /// <summary>
    /// A .NET based unit test class. Demonstrating unit testing and TDD centric practices.
    /// </summary>
    public static class BlogServiceTests
    {
        public static async Task RunAll()
        {
            await Run(nameof(GetPosts_returns_posts_sorted_newest_first), GetPosts_returns_posts_sorted_newest_first);
            await Run(nameof(GetPostById_returns_the_right_post), GetPostById_returns_the_right_post);
            await Run(nameof(AddComment_assigns_next_id_and_saves), AddComment_assigns_next_id_and_saves);
            await Run(nameof(AddReply_assigns_next_id_and_saves), AddReply_assigns_next_id_and_saves);

            Console.WriteLine("All BlogServiceTests are passed");
        }

        private static async Task Run(string name, Func<Task> test)
        {
            try
            {
                await test();
                Console.WriteLine($"PASS: {name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAIL: {name}");
                Console.WriteLine(ex);
                throw;
            }
        }

        private static async Task GetPosts_returns_posts_sorted_newest_first()
        {
            var doc = DocWithTwoPosts(out var older, out var newer);
            var (service, _) = CreateService(doc);

            var posts = await service.GetPostsAsync();

            Expect.Equal(2, posts.Count, "returns 2 posts");
            Expect.Equal(newer.Id, posts[0].Id, "first is newest");
            Expect.Equal(older.Id, posts[1].Id, "second is older");
        }

        private static async Task GetPostById_returns_the_right_post()
        {
            var doc = DocWithTwoPosts(out var older, out _);
            var (service, _) = CreateService(doc);

            var post = await service.GetPostByIdAsync(older.Id);

            Expect.NotNull(post, "post exists");
            Expect.Equal(older.Id, post!.Id, "ids match");
        }

        private static async Task AddComment_assigns_next_id_and_saves()
        {
            var doc = DocWithOnePostAndOneComment(out var postId, existingCommentId: 7);
            var (service, repo) = CreateService(doc);

            var comment = new CommentItem
            {
                Name = "Alice",
                EmailAddress = "alice@example.com",
                Message = "Hello",
                Date = DateTime.UtcNow,
                Attachments = new List<CommentAttachmentItem>(),
                Replies = new List<CommentReplyItem>()
            };

            await service.AddCommentAsync(postId, comment);

            Expect.Equal(8, comment.Id, "comment id increments");

            var saved = await repo.GetAsync();
            var post = saved.BlogPosts.Single(p => p.Id == postId);

            Expect.Equal(2, post.Comments.Count, "comment added");
            Expect.True(post.Comments.Any(c => c.Id == 8), "new comment is present");
        }

        private static async Task AddReply_assigns_next_id_and_saves()
        {
            var doc = DocWithOnePostOneCommentAndOneReply(out var postId, out var commentId, existingReplyId: 3);
            var (service, repo) = CreateService(doc);

            var reply = new CommentReplyItem
            {
                Name = "Bob",
                EmailAddress = "bob@example.com",
                Message = "Reply here",
                Date = DateTime.UtcNow
            };

            await service.AddReplyAsync(postId, commentId, reply);

            Expect.Equal(4, reply.Id, "reply id increments");

            var saved = await repo.GetAsync();
            var post = saved.BlogPosts.Single(p => p.Id == postId);
            var comment = post.Comments.Single(c => c.Id == commentId);

            Expect.Equal(2, comment.Replies.Count, "reply added");
            Expect.True(comment.Replies.Any(r => r.Id == 4), "new reply is present");
        }

        // simple setup
        private static (BlogService service, InMemoryRepository repo) CreateService(BlogPostDocument doc)
        {
            var repo = new InMemoryRepository(doc);
            var cache = new MemoryCache(new MemoryCacheOptions()); // real cache
            var service = new BlogService(repo, cache);
            return (service, repo);
        }

        private sealed class InMemoryRepository : IRepository<BlogPostDocument>
        {
            private BlogPostDocument _doc;

            public InMemoryRepository(BlogPostDocument initial) => _doc = initial;

            public Task<BlogPostDocument> GetAsync() => Task.FromResult(_doc);

            public Task SaveAsync(BlogPostDocument value)
            {
                _doc = value;
                return Task.CompletedTask;
            }
        }

        // moq data
        private static BlogPostDocument DocWithTwoPosts(out BlogPostItem older, out BlogPostItem newer)
        {
            older = new BlogPostItem
            {
                Id = 1,
                Title = "Old",
                Date = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Comments = new List<CommentItem>()
            };

            newer = new BlogPostItem
            {
                Id = 2,
                Title = "New",
                Date = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Comments = new List<CommentItem>()
            };

            return new BlogPostDocument { BlogPosts = new List<BlogPostItem> { older, newer } };
        }

        private static BlogPostDocument DocWithOnePostAndOneComment(out int postId, int existingCommentId)
        {
            postId = 10;

            return new BlogPostDocument
            {
                BlogPosts = new List<BlogPostItem>
                {
                    new BlogPostItem
                    {
                        Id = postId,
                        Title = "Post",
                        Date = DateTime.UtcNow,
                        Comments = new List<CommentItem>
                        {
                            new CommentItem
                            {
                                Id = existingCommentId,
                                Name = "Existing",
                                EmailAddress = "e@example.com",
                                Message = "Existing comment",
                                Date = DateTime.UtcNow,
                                Attachments = new List<CommentAttachmentItem>(),
                                Replies = new List<CommentReplyItem>()
                            }
                        }
                    }
                }
            };
        }

        private static BlogPostDocument DocWithOnePostOneCommentAndOneReply(out int postId, out int commentId, int existingReplyId)
        {
            postId = 20;
            commentId = 5;

            return new BlogPostDocument
            {
                BlogPosts = new List<BlogPostItem>
                {
                    new BlogPostItem
                    {
                        Id = postId,
                        Title = "Post",
                        Date = DateTime.UtcNow,
                        Comments = new List<CommentItem>
                        {
                            new CommentItem
                            {
                                Id = commentId,
                                Name = "Commenter",
                                EmailAddress = "c@example.com",
                                Message = "Comment",
                                Date = DateTime.UtcNow,
                                Attachments = new List<CommentAttachmentItem>(),
                                Replies = new List<CommentReplyItem>
                                {
                                    new CommentReplyItem
                                    {
                                        Id = existingReplyId,
                                        Name = "Existing replier",
                                        EmailAddress = "r@example.com",
                                        Message = "Existing reply",
                                        Date = DateTime.UtcNow
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        // ----------
        // assertions
        // ----------

        private static class Expect
        {
            public static void True(bool condition, string because)
            {
                if (!condition) throw new Exception("Expected TRUE because " + because);
            }

            public static void NotNull(object? value, string because)
            {
                if (value is null) throw new Exception("Expected NOT NULL because " + because);
            }

            public static void Equal<T>(T expected, T actual, string because)
            {
                if (!EqualityComparer<T>.Default.Equals(expected, actual))
                    throw new Exception($"Expected {expected} but got {actual} because {because}");
            }
        }
    }
}

