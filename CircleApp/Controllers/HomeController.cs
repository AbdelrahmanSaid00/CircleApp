using CircleApp.Data.Models;
using CircleApp.Services.Interfaces;
using CircleApp.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CircleApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPostService _postService;
        private readonly IStoryService _storyService;
        private readonly IHashtagService _hashtagService;
        private readonly IFileService _fileService;
        private readonly IFavoriteService _favoriteService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IPostService postService,
            IStoryService storyService,
            IHashtagService hashtagService,
            IFileService fileService,
            IFavoriteService favoriteService,
            ILogger<HomeController> logger)
        {
            _postService = postService;
            _storyService = storyService;
            _hashtagService = hashtagService;
            _fileService = fileService;
            _favoriteService = favoriteService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            int loggedInUserId = 1;
            var allPosts = await _postService.GetAllPostsAsync(loggedInUserId);
            return View(allPosts);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(PostVM post)
        {
            int loggedInUserId = 1;
            await _postService.CreatePostAsync(post, loggedInUserId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostLike(PostLIikeVM postLIikeVM)
        {
            int loggedInUserId = 1;
            await _postService.TogglePostLikeAsync(postLIikeVM.PostId, loggedInUserId);
            return RedirectToAction("Index");
        }

        //[HttpPost]
        //public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM postFavoriteVM)
        //{
        //    int loggedInUserId = 1;
        //    await _postService.TogglePostFavoriteAsync(postFavoriteVM.PostId, loggedInUserId);
        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        public async Task<IActionResult> TogglePostVisibility(PostVisibilityVM postVisibilityVM)
        {
            int loggedInUserId = 1;
            await _postService.TogglePostVisibilityAsync(postVisibilityVM.PostId, loggedInUserId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddPostComment(PostCommentVM postCommentVM)
        {
            int loggedInUserId = 1;
            await _postService.AddPostCommentAsync(postCommentVM, loggedInUserId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddPostReport(PostReportVM postReportVM)
        {
            int loggedInUserId = 1;
            await _postService.AddPostReportAsync(postReportVM, loggedInUserId);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
            {
                return RedirectToAction("Index");
            }
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> RemovePostComment(RemoveCommentVM removeCommentVM)
        {
            await _postService.RemovePostCommentAsync(removeCommentVM.CommentId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SoftDeletePost(int postId)
        {
            await _postService.SoftDeletePostAsync(postId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> HardDeletePost(int postId)
        {
            await _postService.HardDeletePostAsync(postId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> PostDelete(int postId, string deleteType = "soft")
        {
            if (string.Equals(deleteType, "hard", StringComparison.OrdinalIgnoreCase))
            {
                return await HardDeletePost(postId);
            }
            return await SoftDeletePost(postId);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStory(CreateStoryVM model)
        {
            if (model.Image != null && model.Image.Length > 0)
            {
                int loggedInUserId = 1;
                string? relativePath = await _fileService.UploadFileAsync(model.Image, "images/stories");
                if (!string.IsNullOrEmpty(relativePath))
                {
                    var newStory = new Story
                    {
                        ImageUrl = relativePath,
                        DateCreated = DateTime.Now,
                        UserId = loggedInUserId
                    };
                    await _storyService.AddStoryAsync(newStory);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStory(int storyId)
        {
            await _storyService.DeleteStoryAsync(storyId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Hashtag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return RedirectToAction("Index");
            }

            string normalizedTag = tag.StartsWith("#") ? tag : "#" + tag;
            ViewBag.Hashtag = normalizedTag;

            var posts = await _hashtagService.GetPostsByHashtagAsync(normalizedTag);
            return View(posts);
        }
        [HttpPost]
        public async Task<IActionResult> TogglePostFavorite (int postId)
        {
            int loggedInUserId = 1;
            await _favoriteService.ToggleAsync(postId, loggedInUserId);
            bool isFavorite = await _favoriteService.IsFavoriteAsync(postId , loggedInUserId);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> MyFavorites ()
        {
            int logedInUserId = 1;
            var favoritePosts = await _favoriteService.GetFavoritePostsForUserAsync(logedInUserId);
            return View(favoritePosts);
        }
    }
}
