using CircleApp.Data;
using CircleApp.Data.Models;
using CircleApp.Services.Interfaces;
using CircleApp.ViewModels.Home;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CircleApp.Services.Implementations
{
    public class PostService : IPostService
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
        private readonly IHashtagService _hashtagService;

        public PostService(AppDbContext context, IFileService fileService, IHashtagService hashtagService)
        {
            _context = context;
            _fileService = fileService;
            _hashtagService = hashtagService;
        }

        public async Task<List<Post>> GetAllPostsAsync(int currentUserId)
        {
            return await _context.posts
                .Where(p => (!p.IsPrivate || p.UserId == currentUserId) && p.Reports.Count < 5)
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Favorites)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .Include(p => p.Reports)
                .OrderByDescending(p => p.DataCreated)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task CreatePostAsync(PostVM postVM, int userId)
        {
            string? imagePath = await _fileService.UploadFileAsync(postVM.Image, "images/posts");

            var newPost = new Post
            {
                Content = postVM.Content,
                ImageUrl = imagePath,
                UserId = userId,
                DataCreated = DateTime.Now,
                DataUpdated = DateTime.Now,
                IsPrivate = false,
                NrOfReports = 0,
                IsDeleted = false
            };

            await _context.posts.AddAsync(newPost);
            await _context.SaveChangesAsync();

            await _hashtagService.ProcessHashtagsForPostAsync(newPost);
        }

        public async Task TogglePostLikeAsync(int postId, int userId)
        {
            var existingLike = await _context.likes
                .FirstOrDefaultAsync(l => l.postId == postId && l.userId == userId);

            if (existingLike != null)
            {
                _context.likes.Remove(existingLike);
            }
            else
            {
                var newLike = new Like
                {
                    postId = postId,
                    userId = userId
                };
                await _context.likes.AddAsync(newLike);
            }

            await _context.SaveChangesAsync();
        }

        public async Task TogglePostFavoriteAsync(int postId, int userId)
        {
            var existingFav = await _context.favorites
                .FirstOrDefaultAsync(f => f.postId == postId && f.userId == userId);

            if (existingFav != null)
            {
                _context.favorites.Remove(existingFav);
            }
            else
            {
                var newFav = new Favorite
                {
                    postId = postId,
                    userId = userId,
                    DateCreate = DateTime.Now
                };
                await _context.favorites.AddAsync(newFav);
            }

            await _context.SaveChangesAsync();
        }

        public async Task TogglePostVisibilityAsync(int postId, int userId)
        {
            var post = await _context.posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post != null)
            {
                post.IsPrivate = !post.IsPrivate;
                post.DataUpdated = DateTime.Now;
                _context.posts.Update(post);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddPostCommentAsync(PostCommentVM commentVM, int userId)
        {
            var comment = new Comment
            {
                postId = commentVM.PostId,
                userId = userId,
                Content = commentVM.Content,
                DateCreated = DateTime.Now,
                DateUpdate = DateTime.Now
            };

            await _context.comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task AddPostReportAsync(PostReportVM reportVM, int userId)
        {
            var existingReport = await _context.reports
                .FirstOrDefaultAsync(r => r.postId == reportVM.PostId && r.userId == userId);

            if (existingReport == null)
            {
                var report = new Report
                {
                    postId = reportVM.PostId,
                    userId = userId,
                    DateCreate = DateTime.Now
                };

                await _context.reports.AddAsync(report);

                var post = await _context.posts.FirstOrDefaultAsync(p => p.Id == reportVM.PostId);
                if (post != null)
                {
                    post.NrOfReports++;
                    _context.posts.Update(post);
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task RemovePostCommentAsync(int commentId)
        {
            var comment = await _context.comments.FirstOrDefaultAsync(c => c.Id == commentId);
            if (comment != null)
            {
                _context.comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SoftDeletePostAsync(int postId)
        {
            var post = await _context.posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post != null)
            {
                post.IsDeleted = true;
                post.DeletedAt = DateTime.Now;
                _context.posts.Update(post);
                await _context.SaveChangesAsync();

                await _hashtagService.RemoveHashtagsForPostAsync(postId);
            }
        }

        public async Task HardDeletePostAsync(int postId)
        {
            var post = await _context.posts
                .IgnoreQueryFilters()
                .Include(p => p.Hashtags)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post != null)
            {
                if (!string.IsNullOrEmpty(post.ImageUrl))
                {
                    _fileService.DeleteFile(post.ImageUrl);
                }

                await _hashtagService.RemoveHashtagsForPostAsync(postId);
                _context.posts.Remove(post);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Post> GetPostByIdAsync(int postId)
        {
            return await _context.posts
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Favorites)
                .Include(p => p.Hashtags)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .Include(p => p.Reports)
                .ThenInclude(r => r.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == postId);
        }
    }
}
