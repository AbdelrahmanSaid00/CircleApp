using CircleApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CircleApp.ViewComponents
{
    public class StoriesViewComponent : ViewComponent
    {
        private readonly IStoryService _storyService;

        public StoriesViewComponent(IStoryService storyService)
        {
            _storyService = storyService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var activeStories = await _storyService.GetActiveStoriesAsync();
            return View(activeStories);
        }
    }
}
