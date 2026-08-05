using CircleApp.Data;
using CircleApp.Data.Models;
using CircleApp.Services.Interfaces;
using CircleApp.ViewModels.Home;
using Microsoft.EntityFrameworkCore;

namespace CircleApp.Services.Implementations
{
    public class FavoriteService : IFavoriteService
    {
        private readonly AppDbContext _db;
        public FavoriteService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<List<Post>> GetFavoritePostsForUserAsync(int userId)
        {
            var posts = await _db.posts
                .Where(p => p.Favorites.Any(f => f.userId == userId))
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Favorites)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .Include(p => p.Reports)
                .OrderByDescending(p => p.DataCreated)
                .AsNoTracking()
                .ToListAsync();
            return posts;
        }

        public async Task<bool> IsFavoriteAsync(int postId, int userId)
        {
            return await _db.favorites.AnyAsync(f => f.userId == userId && f.postId == postId);
        }

        public async Task ToggleAsync(int postId, int userId)
        {
            var favorite = await _db.favorites.FirstOrDefaultAsync(f => f.userId == userId && f.postId == postId);
            if (favorite != null)
            {
                _db.favorites.Remove(favorite);
            }
            else
            {
                var newFavorite = new Favorite
                {
                    userId = userId,
                    postId = postId,
                    DateCreate = DateTime.Now
                };
                await _db.favorites.AddAsync(newFavorite);
            }
            await _db.SaveChangesAsync();
        }
    }
}
