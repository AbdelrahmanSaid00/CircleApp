using CircleApp.Data.Models;
using CircleApp.ViewModels.Home;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CircleApp.Services.Interfaces
{
    public interface IHashtagService
    {
        Task ProcessHashtagsForPostAsync(Post post);
        Task RemoveHashtagsForPostAsync(int postId);
        Task<List<HashtagVM>> GetTrendingHashtagsAsync();
        Task<List<Post>> GetPostsByHashtagAsync(string tag);
    }
}
