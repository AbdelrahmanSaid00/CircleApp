using CircleApp.Data;
using CircleApp.Data.Models;
using CircleApp.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CircleApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;
        public HomeController(AppDbContext context , ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            var allPosts = await _context.posts
                .Include(p => p.User) 
                .Include(p => p.Likes)
                .Include(p => p.Favorites)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .OrderByDescending(n => n.DataCreated)
                .ToListAsync();
            return View(allPosts);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost (PostVM post)
        {
            // Get the logged in user
            int loggedInUserId = 1;
            var newPost = new Post
            {
                Content = post.Content,
                DataCreated = DateTime.Now,
                DataUpdated = DateTime.Now,
                ImageUrl = "",
                NrOfReports = 0,
                UserId = loggedInUserId
            };

            // Check and save the image
            if (post.Image != null && post.Image.Length > 0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (post.Image.ContentType.Contains("image"))
                {
                    string rootFolderPathImages = Path.Combine(rootFolderPath, "images/uploaded");
                    Directory.CreateDirectory(rootFolderPathImages);

                    string FileName = Guid.NewGuid().ToString() + Path.GetExtension(post.Image.FileName);
                    string FilePath = Path.Combine(rootFolderPathImages, FileName);

                    using (var stream = new FileStream(FilePath, FileMode.Create))
                    {
                        await post.Image.CopyToAsync(stream);
                    }
                    // Set the URL to the newPost object
                    newPost.ImageUrl = "/images/uploaded/" + FileName;
                }
            }

            await _context.posts.AddAsync(newPost);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> TogglePostLike (PostLIikeVM postLIikeVM)
        {
            int loggedInUserId = 1;
            
            //check if the user has already liked the post
            var like = await _context.likes
                .Where(l => l.postId == postLIikeVM.PostId && l.userId == loggedInUserId)
                .FirstOrDefaultAsync();
            if (like != null)
            {
                _context.likes.Remove(like);
                await _context.SaveChangesAsync();
            }
            else
            {
                var newLike = new Like
                {
                    postId = postLIikeVM.PostId,
                    userId = loggedInUserId
                };
                await _context.likes.AddAsync(newLike);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM postFavoriteVM)
        {
            int loggedInUserId = 1;

            //check if the user has already favorite the post
            var favorite = await _context.favorites
                .Where(l => l.postId == postFavoriteVM.PostId && l.userId == loggedInUserId)
                .FirstOrDefaultAsync();
            if (favorite != null)
            {
                _context.favorites.Remove(favorite);
                await _context.SaveChangesAsync();
            }
            else
            {
                var newFavorite = new Favorite
                {
                    postId = postFavoriteVM.PostId,
                    userId = loggedInUserId
                };
                await _context.favorites.AddAsync(newFavorite);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> AddPostComment (PostCommentVM postCommentVM)
        {
            int loggedUserId = 1;
            //Create a Post Object
            var newComment = new Comment()
            {
                postId = postCommentVM.PostId,
                userId = loggedUserId,
                Content = postCommentVM.Content,
                DateCreated = DateTime.Now,
                DateUpdate = DateTime.Now,
            };
            await _context.comments.AddAsync(newComment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> RemovePostComment (RemoveCommentVM removeCommentVM)
        {
            var comment = await _context.comments.FirstOrDefaultAsync(c => c.Id == removeCommentVM.CommentId);
            if (comment != null)
            {
                _context.comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
