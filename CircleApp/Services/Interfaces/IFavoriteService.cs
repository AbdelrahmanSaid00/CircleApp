using CircleApp.Data.Models;
using CircleApp.ViewModels.Home;

namespace CircleApp.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task ToggleAsync(int postId, int userId);
        Task<List<Post>> GetFavoritePostsForUserAsync(int userId);
        Task<bool> IsFavoriteAsync(int postId, int userId);
    }
}
