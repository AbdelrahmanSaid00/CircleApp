using CircleApp.Data.Models;
using CircleApp.ViewModels.Home;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CircleApp.Services.Interfaces
{
    public interface IPostService
    {
        Task<List<Post>> GetAllPostsAsync(int currentUserId);
        Task<Post> GetPostByIdAsync(int postId);
        Task CreatePostAsync(PostVM postVM, int userId);
        Task TogglePostLikeAsync(int postId, int userId);
        Task TogglePostFavoriteAsync(int postId, int userId);
        Task TogglePostVisibilityAsync(int postId, int userId);
        Task AddPostCommentAsync(PostCommentVM commentVM, int userId);
        Task AddPostReportAsync(PostReportVM reportVM, int userId);
        Task RemovePostCommentAsync(int commentId);
        Task SoftDeletePostAsync(int postId);
        Task HardDeletePostAsync(int postId);
    }
}
