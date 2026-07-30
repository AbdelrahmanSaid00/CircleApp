using CircleApp.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CircleApp.Services.Interfaces
{
    public interface IStoryService
    {
        Task<List<Story>> GetActiveStoriesAsync();
        Task<Story?> GetStoryByIdAsync(int id);
        Task AddStoryAsync(Story story);
        Task DeleteStoryAsync(int id);
    }
}
